using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExampleRenderPipeline : RenderPipeline
{

    public ExampleRenderPipeline()
    {

    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        // Create and schedule a command to clear the current render target
        var cmd = new CommandBuffer();
        cmd.ClearRenderTarget(true, true, Color.black);
        context.ExecuteCommandBuffer(cmd);
        cmd.Release();

        // Iterate over all Cameras
        foreach (Camera camera in cameras)
        {
			// Get the culling parameters from the current Camera
			camera.TryGetCullingParameters(out var cullingParameters);

			// Use the culling parameters to perform a cull operation, and store the results
			var cullingResults = context.Cull(ref cullingParameters);

			// Update the value of built-in shader variables, based on the current Camera
			context.SetupCameraProperties(camera);

			// Tell Unity which geometry to draw, based on its LightMode Pass tag value
			ShaderTagId shaderTagId = new ShaderTagId("ExampleLightModeTag");

			// Tell Unity how to sort the geometry, based on the current Camera
			var sortingSettings = new SortingSettings(camera);


            // Create a DrawingSettings struct that describes which geometry to draw and how to draw it
            DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);

	        // Tell Unity how to filter the culling results, to further specify which geometry to draw
            // Use FilteringSettings.defaultValue to specify no filtering
            FilteringSettings filteringSettings = FilteringSettings.defaultValue;

			// Schedule a command to draw the geometry, based on the settings you have defined
			context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

			// Schedule a command to draw the Skybox if required
			if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
			{
				context.DrawSkybox(camera);
			}

			// Instruct the graphics API to perform all scheduled commands
			context.Submit();
        }
    }
}
