using System;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input; 


namespace Fusee.Tutorial.Core
{

    [FuseeApplication(Name = "Tutorial Example", Description = "The official FUSEE Tutorial.")]
    public class Tutorial : RenderCanvas
    {
        private IShaderParam _alphaParam;
        private float _alpha;
        private Mesh _mesh;

        private const string _vertexShader = @"
            attribute vec3 fuVertex;
            uniform float alpha;
            varying vec3 modelpos;

            void main()
            {                
                modelpos = fuVertex;
                float s = sin(alpha);
                float c = cos(alpha);
                
                gl_Position = vec4( fuVertex.x * c - fuVertex.z * s, 
                                    fuVertex.y, 
                                    fuVertex.x * s + fuVertex.z * c,                       
                                    1.0);
            }";

        private const string _pixelShader = @"
            #ifdef GL_ES
                precision highp float;
            #endif
            varying vec3 modelpos;

            void main()
            {
                gl_FragColor = vec4(modelpos*0.5 + 0.5, 1);
            }";


        // Init is called on startup. 
        public override void Init()
        {

            _mesh = new Mesh
            {
                Vertices = new[]
                {
                    new float3(-0.8165f, -0.3333f, -0.4714f), // Vertex 0
                    new float3(0.8165f, -0.3333f, -0.4714f),  // Vertex 1
                    new float3(0, -0.3333f, 0.9428f),         // Vertex 2
                    new float3(0, 1, 0),                      // Vertex 3
                },
                Triangles = new ushort[]
                {
                    0, 2, 1,  // Triangle 0 "Bottom" facing towards negative y axis
                    0, 1, 3,  // Triangle 1 "Back side" facing towards negative z axis
                    1, 2, 3,  // Triangle 2 "Right side" facing towards positive x axis
                    2, 0, 3,  // Triangle 3 "Left side" facing towrads negative x axis
                },
            };

            var shader = RC.CreateShader(_vertexShader, _pixelShader);
            RC.SetShader(shader);
            // Set the clear color for the backbuffer.
            _alphaParam = RC.GetShaderParam(shader, "alpha");
            _alpha = 0;
            RC.ClearColor = new float4(0.1f, 0.3f, 0.2f, 1);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            float2 speed_1 = Mouse.Velocity;
            float2 speed_2 = new float2(Keyboard.LeftRightAxis, Keyboard.UpDownAxis); 
            _alpha += speed_2.x * 0.01f;
            RC.SetShaderParam(_alphaParam, _alpha);
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Render(_mesh);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width/(float) Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(3.141592f * 0.25f, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }

    }
}