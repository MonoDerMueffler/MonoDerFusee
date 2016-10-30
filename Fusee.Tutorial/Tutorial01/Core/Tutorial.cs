using System;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;


namespace Fusee.Tutorial.Core
{
    [FuseeApplication(Name = "Tutorial Example", Description = "The official FUSEE Tutorial.")]
    public class Tutorial : RenderCanvas
    {
        //create shader
        private ShaderProgram _shader_1;
        private ShaderProgram _shader_2;
        //create mesh variable
        private Mesh _mesh_1;
        private Mesh _mesh_2;
        //create Shader variables and fill them with a string
        private const string _vertexShader_1 = @"
            attribute vec3 fuVertex;

            void main(){
                gl_Position = vec4(fuVertex, 1.0);
            }";

        private const string _pixelShader_1 = @"
            #ifdef GL_ES
            precision highp float;
            #endif

            void main(){
                gl_FragColor = vec4(1, 0, 1, 1);
            }";
        private const string _vertexShader_2 = @"
            attribute vec3 fuVertex;

            void main(){
                gl_Position = vec4(fuVertex, 1.0);
            }";

        private const string _pixelShader_2 = @"
            #ifdef GL_ES
            precision highp float;
            #endif

            void main(){
                gl_FragColor = vec4(1, 0, 0, 1);
            }";


        // Init is called on startup. 
        public override void Init()
        {

            // Set the clear color for the backbuffer to light green.
            RC.ClearColor = new float4(0.5f, 1, 0.7f, 1);
            _shader_1 = RC.CreateShader(_vertexShader_1, _pixelShader_1);
            _shader_2 = RC.CreateShader(_vertexShader_2, _pixelShader_2);
            
            _mesh_1 = new Mesh
            {
                //generate vertecies
                Vertices = new[]
                {
                    new float3(-0.75f, -0.75f, 0),
                    new float3(0.75f, -0.75f, 0),
                    new float3(0, 0.75f, 0),
                },

                Triangles = new ushort[] { 0, 1, 2},

            };
            
            _mesh_2 = new Mesh
            {
                //generate vertecies
                Vertices = new[]
                {
                    new float3(-1, -1, -1.1f),
                    new float3(0, -1, -1.1f),
                    new float3(1, 1, 2),
                },

                Triangles = new ushort[] { 0, 1, 2 },

            };
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            RC.SetShader(_shader_1);
            RC.Render(_mesh_1);
            RC.SetShader(_shader_2);
            RC.Render(_mesh_2);

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