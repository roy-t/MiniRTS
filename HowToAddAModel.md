# How to add a 3D model

*For Blender 2.8:*

1. Create a 3D model as you would normally do. 
2. Double check the normals are facing outwards by going into edit mode, and selecting 'show normals' in the viewport overlays.
3. UV Unwrap the model
4. Create a texture file in `path/to/model/textures`
5. Add a texture.
6. Add a material, in the node view set the Base Color to reference the texture you've just created.
7. Verify the material/texture work by setting the viewport shading to 'display render preview' or by rendering the image. 
8. Export the file as FBX store it in the same folder as you've stored the model so all relative paths are preserved.
9. Copy the model to the content directory, copy the texture to `/path/to/model/in/content/directory/textures`.
10. Create the file `<modelname>.ini` and fill it with the material description (see other files for an example)
11. Add the model to MonoGame's content tool and set the processor to the multi-material model processor.
12. You can now use the model in game!