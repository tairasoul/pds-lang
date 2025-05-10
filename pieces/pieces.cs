namespace tairasoul.pdsl.pieces;

public class Coordinate
{
    public int x { get; set; }
    public int y { get; set; }
}


public static class Area
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class Comment
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class ComputerControl
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionBlock
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 33;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 22 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionCoordinate
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 33;
    public static Coordinate Coordinate1 { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Coordinate2 { get; set; } = new Coordinate { x = 15, y = 11 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 22 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionEntity
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 33;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text1 { get; set; } = new Coordinate { x = 15, y = 11 };
    public static Coordinate Text2 { get; set; } = new Coordinate { x = 15, y = 22 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionFluid
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 33;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate FluidFilter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 22 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionItems
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 33;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 22 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionLight
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionPressure
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionRedstone
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ConditionRf
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class CoordinateOperator
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Coordinate { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class Crafting
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 33;
    public static Coordinate Filter1 { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter2 { get; set; } = new Coordinate { x = 15, y = 11 };
    public static Coordinate Filter3 { get; set; } = new Coordinate { x = 15, y = 22 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class Dig
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class DroneConditionEntity
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Text1 { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text2 { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class DroneConditionFluid
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate FluidFilter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class DroneConditionItems
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class DroneConditionPressure
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class DroneConditionRf
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class DroneConditionUpgrades
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class DropItems
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class EditSign
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
}


public static class EmitRedstone
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class EntityAttack
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ExportFluid
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate FluidFilter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ExportRf
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ExportToInventory
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ExternalProgram
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class FluidFilter
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class ForEachCoordinate
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ForEachItem
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class GoToLocation
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ImportEntity
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ImportFluid
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate FluidFilter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ImportFromInventory
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class ImportRf
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class ItemAssignment
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class ItemFilter
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class Jump
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class JumpSubroutine
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class Label
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class Logistics
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class PickUpItems
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class Place
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class RenameDrone
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class RightClick
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class RightClickEntity
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 22;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 11 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class Standby
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class Start
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class Suicide
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class TeleportToLocation
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Area { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class Text
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}


public static class VoidFluid
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate FluidFilter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class VoidItem
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Filter { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "left", "right" };
}


public static class Wait
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}
