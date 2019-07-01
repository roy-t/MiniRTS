import bpy
import primitives
import random
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
        col.label("Scripts:")
        col.operator("mesh.make_tetrahedron", text="Add Tetrahedron")
        col.operator("mesh.make_turret", text="Add Turret")

        col2 = layout.column(align=True)
        col2.label("Properties:")
        col2.prop(context.scene, "seed")
        col2.operator("editor.new_seed", text="New Seed")
    # end draw

# end ProceduralGenerationPanel


class NewSeed(bpy.types.Operator):
    bl_idname = "editor.new_seed"
    bl_label = "New Seed"

    def invoke(self, context, event):
        context.scene.seed = random.randint(0, pow(2, 31))
        return {"FINISHED"}
    # end invoke

# end NewSeed


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
    bl_label = "Add Turret"
    bl_options = {"UNDO"}

    def invoke(self, context, event):
        self.report({'INFO'}, "Building turret with seed: {0}"
                    .format(context.scene.seed))
        primitives.createTurret(context.scene.seed)        
        return {"FINISHED"}
    # end invoke

# end MakeTetrahedron


classes = {ProceduralGenerationPanel, NewSeed, MakeTetrahedron, MakeTurret}
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
