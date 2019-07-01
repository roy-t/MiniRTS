import bpy
import primitives
# Inspired by: https://github.com/a1studmuffin/SpaceshipGenerator


class ProceduralGenerationPanel(bpy.types.Panel):
    """Panel in create tab of the tool shelf of the 3D view"""
    bl_space_type = "VIEW_3D"
    bl_region_type = "TOOLS"
    bl_context = "objectmode"
    bl_category = "Create"
    bl_label = "Procedural Generation"

    bpy.types.Scene.seed = bpy.props.IntProperty(name="Seed")

    def draw(self, context):
        layout = self.layout
        col = layout.column(align=True)
        col.operator("mesh.make_tetrahedron", text="Add Tetrahedron")
        col.operator("mesh.make_turret", text="Add Turret")        
        
        layout.prop(context.scene, "seed")
    # end draw

# end ProceduralGenerationPanel


class MakeTetrahedron(bpy.types.Operator):
    bl_idname = "mesh.make_tetrahedron"
    bl_label = "Add Tetrahedron"
    bl_options = {"UNDO"}        

    def invoke(self, context, event):
        mesh = primitives.createTetrahdron()
        context.scene.objects.link(mesh)
        return {"FINISHED"}
    # end invoke

# end MakeTetrahedron


class MakeTurret(bpy.types.Operator):
    bl_idname = "mesh.make_turret"
    bl_label = "Add Tu2rret"
    bl_options = {"UNDO"}

    def invoke(self, context, event):
        self.report({'INFO'}, "Building turret with seed: {0}"
                    .format(context.scene.seed))
        mesh = primitives.createTurret(context.scene.seed)
        context.scene.objects.link(mesh)
        return {"FINISHED"}
    # end invoke

# end MakeTetrahedron


classes = {ProceduralGenerationPanel, MakeTetrahedron, MakeTurret}
dependencies = {primitives}


def register():
    for c in classes:
        bpy.utils.register_class(c)


def unregister():
    for c in classes:
        bpy.utils.unregister_class(c)


def reload_dependencies():
    """Reload modules that this module depends on"""
    import importlib
    for d in dependencies:
        importlib.reload(d)
