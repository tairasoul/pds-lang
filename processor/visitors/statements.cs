using pieces = tairasoul.pdsl.pieces;
using tairasoul.pdsl.visitors.returnTypes;
using expression = tairasoul.pdsl.ast.expressions.Expression;
using statement = tairasoul.pdsl.ast.statements.Statement;
using tairasoul.pdsl.lexer;
using tairasoul.pdsl.processor;
using tairasoul.pdsl.visitors.externs;
using tairasoul.pdsl.visitors.errors;

namespace tairasoul.pdsl.visitors.statements;

static class CommentVisitor 
{
  public static VisitorReturn process(pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.Comment comment, ref pieces.Coordinate[] CommentPositions) 
  {
    int[] sameY_xValues = [..CommentPositions.Where((coord) => coord.y == lastPos.y - 8 + pieces.Comment.y).Select((coord) => coord.x)];
    bool foundDuplicate = false;
    int currentXOffset = 40;
    int traversed = 2;
    int multForNonDuplicate = 0;
    foreach (int xValue in sameY_xValues) {
      traversed++;
      if (foundDuplicate) 
      {
        multForNonDuplicate++;
      }
      if (xValue == lastPos.x - currentXOffset) 
      {
        foundDuplicate = true;
      }
    }
    multForNonDuplicate = traversed;
    currentXOffset += 40 * multForNonDuplicate;
    CommentPositions = [..CommentPositions, new() { x = lastPos.x - currentXOffset, y = lastPos.y - 8 + pieces.Comment.y }];
    Widget commentWidget = new("pneumaticcraft:comment", new pieces.Coordinate { x = lastPos.x - currentXOffset, y = lastPos.y - 8 + pieces.Comment.y });
    commentWidget.ExtraProperties["string"] = comment.text;
    VisitorReturn ret = new(lastPos, [commentWidget]);
    return ret;
  }
}

static class FunctionStatementVisitor 
{
  public static VisitorReturn process(pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.FunctionStatement function, AstProcessor processor) 
  {
    Token label = function.identifier;
    statement.Block block = function.block;
    Widget labelWidget = new("pneumaticcraft:label", new pieces.Coordinate { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y });
    Widget labelTextWidget = new("pneumaticcraft:text", new pieces.Coordinate { x = processor.currentFunctionPos.x + pieces.Label.Text.x, y = processor.currentFunctionPos.y });
    labelTextWidget.ExtraProperties["string"] = label.lexeme;
    processor.lastSize = new() { x = pieces.Label.x, y = pieces.Label.y };
    VisitorReturn[] visitorReturns = [];
    VisitorReturn initialReturn = new(new() { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y }, [labelWidget, labelTextWidget]);
    visitorReturns = [..visitorReturns, initialReturn];
    foreach (statement statement in block.statements) 
    {
      VisitorReturn? processed = processor.processStatement(statement, true);
      if (processed == null) continue;
      processor.currentFunctionPos.y = processed.setLast.y;
      visitorReturns = [..visitorReturns, processed];
    }
    Widget[][] widgetsMapped = [..visitorReturns.Select((visitorReturn) => visitorReturn.widgets)];
    Widget[] returnedWidgets = [];
    foreach (Widget[] mapped in widgetsMapped) 
    {
      returnedWidgets = [..returnedWidgets, ..mapped];
    }
    processor.currentFunctionPos.x += 2000;
    VisitorReturn finalReturn = new(lastPos, returnedWidgets);
    return finalReturn;
  }
}

static class SubroutineVisitor 
{
  public static VisitorReturn process(pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.Subroutine subroutine) 
  {
    Widget jumpsub = new("pneumaticcraft:jump_sub", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget label = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.JumpSubroutine.Text.x, y = lastPos.y + lastSize.y });
    label.ExtraProperties["string"] = subroutine.label.lexeme;
    VisitorReturn ret = new(new() { x = lastPos.x, y = lastPos.y + lastSize.y }, [jumpsub, label]);
    lastSize = new() { x = pieces.JumpSubroutine.x, y = pieces.JumpSubroutine.y };
    return ret;
  }
}

static class ExternVisitor 
{
  public static VisitorReturn? process(pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.name.lexeme == "start") 
    {
      Widget result = StartExternVisitor.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + lastSize.y }, [result]);
      lastSize = new() { x = pieces.Start.x, y = pieces.Start.y };
      return visitor;
    }
    if (@extern.name.lexeme == "area") 
    {
      throw new VisitorError($"Area is not a valid piece outside of arguments!", @extern);
    }
    if (@extern.name.lexeme == "coordinate") 
    {
      throw new VisitorError($"Coordinate is not a valid piece outside of arguments!", @extern);
    }
    if (@extern.name.lexeme == "filter")
    {
      throw new VisitorError($"Filter is not a valid piece outside of arguments!", @extern);
    }
    if (@extern.name.lexeme == "goto" || @extern.name.lexeme == "teleport" || @extern.name.lexeme == "computer_control") 
    {
      Widget[] result = PrimaryMovementExterns.process(@extern.name.lexeme, lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + lastSize.y }, result);
      lastSize = new() { x = pieces.GoToLocation.x, y = pieces.GoToLocation.y };
      return visitor;
    }
    if (@extern.name.lexeme == "external_program") 
    {
      Widget[] result = ExternalProgramExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + lastSize.y }, result);
      lastSize = new() { x = pieces.ExternalProgram.x, y = pieces.ExternalProgram.y };
      return visitor;
    }
    if (@extern.name.lexeme == "logistics") 
    {
      Widget[] result = LogisticsExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + lastSize.y }, result);
      lastSize = new() { x = pieces.Logistics.x, y = pieces.Logistics.y };
      return visitor;
    }
    if (@extern.name.lexeme == "suicide" || @extern.name.lexeme == "standby") 
    {
      Widget result = NoArgumentExternVisitor.process(@extern.name.lexeme, lastPos, lastSize);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + lastSize.y }, [result]);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "harvest") 
    {
      Widget[] result = HarvestExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x,  y = lastPos.y + pieces.Dig.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "dig") 
    {
      Widget[] result = DigExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x,  y = lastPos.y + pieces.Dig.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "place") 
    {
      Widget[] result = PlaceExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x,  y = lastPos.y + pieces.Dig.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "right_click_block") 
    {
      Widget[] result = BlockRightClickExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.RightClick.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "right_click_entity" || @extern.name.lexeme == "export_entity" || @extern.name.lexeme == "import_entity" || @extern.name.lexeme == "edit_sign") 
    {
      string name = @extern.name.lexeme == "right_click_entity" ? "entity_right_click" : @extern.name.lexeme == "export_entity" ? "entity_export" : @extern.name.lexeme == "import_entity" ? "entity_import" : "edit_sign";
      Widget[] result = PrimaryEntityExterns.process(name, lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.RightClickEntity.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "pickup_item") 
    {
      Widget[] result = ItemPickupExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.PickUpItems.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "drop_item") 
    {
      Widget[] result = ItemDropExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.DropItems.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "void_item") 
    {
      Widget[] result = VoidItemExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.VoidItem.y }, result);
      lastSize = new() { x = pieces.VoidItem.x, y = pieces.VoidItem.y };
      return visitor;
    }
    if (@extern.name.lexeme == "void_fluid" || @extern.name.lexeme == "void_liquid") 
    {
      Widget[] result = VoidFluidExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.VoidFluid.y }, result);
      lastSize = new() { x = pieces.VoidFluid.x, y = pieces.VoidFluid.y };
      return visitor;
    }
    if (@extern.name.lexeme == "import_inventory" || @extern.name.lexeme == "export_inventory") 
    {
      Widget[] result = InventoryExtern.process(@extern.name.lexeme, lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ImportFromInventory.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "import_fluid" || @extern.name.lexeme == "import_liquid") 
    {
      Widget[] result = FluidImportExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ImportFluid.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "export_fluid" || @extern.name.lexeme == "export_liquid") 
    {
      Widget[] result = FluidExportExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ExportFluid.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "import_rf" || @extern.name.lexeme == "export_rf") 
    {
      Widget[] result = RFExterns.process(@extern.name.lexeme, lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ImportRf.y }, result);
      lastSize = new() { x = pieces.ImportRf.x, y = pieces.ImportRf.y };
      return visitor;
    }
    if (@extern.name.lexeme == "emit_redstone") 
    {
      Widget[] result = EmitRedstoneExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ImportRf.y }, result);
      lastSize = new() { x = pieces.ImportRf.x, y = pieces.ImportRf.y };
      return visitor;
    }
    if (@extern.name.lexeme == "wait") 
    {
      Widget[] result = WaitExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.Wait.y }, result);
      lastSize = new() { x = pieces.Wait.x, y = pieces.Wait.y };
      return visitor;
    }
    if (@extern.name.lexeme == "coordinate_condition") 
    {
      Widget[] result = CoordinateConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionCoordinate.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "for_each_coordinate") 
    {
      Widget[] result = ForEachCoordinateExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ForEachCoordinate.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "redstone_condition") 
    {
      Widget[] result = RedstoneConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionRedstone.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "light_condition") 
    {
      Widget[] result = LightConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionLight.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "item_inventory_condition") 
    {
      Widget[] result = ItemConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionItems.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "block_condition") 
    {
      Widget[] result = BlockConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionBlock.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "fluid_condition" || @extern.name.lexeme == "liquid_condition") 
    {
      Widget[] result = FluidConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionFluid.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "entity_condition") 
    {
      Widget[] result = EntityConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionEntity.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "pressure_condition") 
    {
      Widget[] result = PressureConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionEntity.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "rf_condition") 
    {
      Widget[] result = RFConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionEntity.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "item_condition") 
    {
      Widget[] result = ItemFilterConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.ConditionEntity.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "drone_item_condition") 
    {
      Widget[] result = DroneItemConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.DroneConditionItems.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "drone_fluid_condition" || @extern.name.lexeme == "drone_liquid_condition") 
    {
      Widget[] result = DroneFluidConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.DroneConditionItems.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "drone_entity_condition") 
    {
      Widget[] result = DroneEntityConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.DroneConditionItems.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "drone_pressure_condition") 
    {
      Widget[] result = DronePressureConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.DroneConditionPressure.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "drone_rf_condition") 
    {
      Widget[] result = DroneRFConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.DroneConditionPressure.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    if (@extern.name.lexeme == "drone_upgrades_condition") 
    {
      Widget[] result = DroneRFConditionExtern.process(lastPos, lastSize, @extern);
      VisitorReturn visitor = new(new() { x = lastPos.x, y = lastPos.y + pieces.DroneConditionItems.y }, result);
      lastSize = new() { x = pieces.Suicide.x, y = pieces.Suicide.y };
      return visitor;
    }
    return null;
  }
}