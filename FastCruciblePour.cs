using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.API.Common;

namespace FastCruciblePour;

public sealed class FastCruciblePourMod : ModSystem
{
    private const string PatchId = "fastcruciblepour";
    private Harmony harmony;

    public override void Start(ICoreAPI api)
    {
        harmony = new Harmony(PatchId);
        var target = AccessTools.Method("Vintagestory.GameContent.BlockSmeltedContainer:OnHeldInteractStep")
            ?? throw new MissingMethodException("Fast Crucible Pour: vanilla pouring method was not found.");
        harmony.Patch(target, transpiler: new HarmonyMethod(typeof(FastCruciblePourMod), nameof(Transpile)));
        api.Logger.Notification("[Fast Crucible Pour] Enabled 50x pouring (100 units per step).");
    }

    public override void Dispose()
    {
        harmony?.UnpatchAll(PatchId);
        base.Dispose();
    }

    public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var code = instructions.ToList();
        var twos = code.Select((instruction, index) => (instruction, index))
            .Where(pair => pair.instruction.LoadsConstant(2)).Select(pair => pair.index).ToArray();

        // Validate the vanilla transfer and accounting pattern before changing anything.
        if (twos.Length != 2 || twos[0] + 4 >= code.Count ||
            code[twos[0] + 3].operand is not MethodInfo min ||
            min.DeclaringType != typeof(Math) || min.Name != nameof(Math.Min) ||
            !code[twos[0] + 4].IsStloc() ||
            !code.Skip(twos[0]).Take(twos[1] - twos[0]).Any(instruction =>
                instruction.operand is MethodInfo method && method.Name == "ReceiveLiquidMetal"))
        {
            throw new InvalidOperationException("Fast Crucible Pour: unsupported pouring implementation; no changes applied.");
        }

        // Remember the requested amount BEFORE the sink overwrites it with the remainder.
        // Using a fixed 100 in the subtraction would lose metal when both the crucible
        // and the mold have less than 100 units available.
        var requested = generator.DeclareLocal(typeof(int));
        code[twos[0]].opcode = OpCodes.Ldc_I4;
        code[twos[0]].operand = 100;
        code[twos[1]].opcode = OpCodes.Ldloc;
        code[twos[1]].operand = requested;
        code.InsertRange(twos[0] + 4, new[]
        {
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Stloc, requested)
        });
        return code;
    }
}
