using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace VoxelTest
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect _waterEffect;
        Effect _terrainEffect;
        Texture2D _terrainTexture;
        ChunkManager _chunkManager;
        bool _fpsMode;
        KeyboardState _lastKeyboardState;
        float _sunLuminosity;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        float _cameraYaw = 0;
        float _cameraPitch = 0;
        Vector3 _cameraPosition;
        Vector3 _cameraSpeed;
        Vector3 _cameraDirection;

        protected override void Initialize()
        {
            base.Initialize();

            _fpsMode = false;
            _sunLuminosity = 1.0f;

            _cameraPosition = new Vector3();
            _cameraDirection = Vector3.Forward;

            _cameraPosition = new Vector3(0, 170, 0);
            _chunkManager = new ChunkManager();
            _chunkManager.GraphicsDevice = GraphicsDevice;
            _chunkManager.Initialize();
        }



        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _terrainEffect = Content.Load<Effect>("TerrainEffect");
            _waterEffect = Content.Load<Effect>("WaterEffect");
            _terrainTexture = Content.Load<Texture2D>("texture");
        }
        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }

            KeyboardState keyboardState = Keyboard.GetState();

            if (_fpsMode)
            {
                MouseState mousestate = Mouse.GetState();
                int decalX = 400 - mousestate.Position.X;
                int decalY = 300 - mousestate.Position.Y;


                _cameraYaw += (float)decalX * 0.001f;
                _cameraPitch += (float)decalY * 0.001f;
                _cameraDirection = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(_cameraYaw, _cameraPitch, 0));
                
                Vector3 bodyDirection = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(_cameraYaw, _cameraPitch, 0));

                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    _cameraPosition += bodyDirection * (0.03f * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    _cameraPosition -= bodyDirection * (0.03f * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
                }

                //_cameraSpeed += Vector3.Down * 0.02f;
                //Vector3 cameraNextPosition = _cameraPosition + _cameraSpeed;
                //Vector3 underCamera = cameraNextPosition + (Vector3.Down * 2.1f);
                //Chunk chunk = _chunkManager.GetChunkAtPosition(underCamera);
                //Point3d underCameraBlocPosition = new Point3d((int)underCamera.X, (int)underCamera.Y, (int)underCamera.Z);
                //if (chunk != null)
                //{
                //    if (chunk.isOpaque(underCameraBlocPosition))
                //    {
                //        _cameraPosition.Y = Math.Min(_cameraPosition.Y + 1.0f, underCameraBlocPosition.Y + 3.05f);
                //        _cameraSpeed = Vector3.Zero;
                //    }
                //}
                //else
                //{
                //    _cameraSpeed = Vector3.Zero;
                //}
                //_cameraPosition += _cameraSpeed;


                Mouse.SetPosition(400, 300);
            }

            if (keyboardState.IsKeyDown(Keys.PageDown))
            {
                _sunLuminosity -= 0.01f;
                _sunLuminosity = Math.Max(Math.Min(_sunLuminosity, 1), 0);
            }
            if (keyboardState.IsKeyDown(Keys.PageUp))
            {
                _sunLuminosity += 0.01f;
                _sunLuminosity = Math.Max(Math.Min(_sunLuminosity, 1), 0);
            }

            if (_lastKeyboardState.IsKeyUp(Keys.F) && keyboardState.IsKeyDown(Keys.F))
            {
                _fpsMode = !_fpsMode;
            }

            if (_lastKeyboardState.IsKeyUp(Keys.J) && keyboardState.IsKeyDown(Keys.J))
            {
                Point3d blocToRemovePosition = new Point3d(15, 134, 15);
                _chunkManager.RemoveBloc(blocToRemovePosition);
            }
            if (_lastKeyboardState.IsKeyUp(Keys.K) && keyboardState.IsKeyDown(Keys.K))
            {
                Point3d blocToAddPosition = new Point3d(15, 134, 15);
                _chunkManager.AddBloc(blocToAddPosition);
            }
            if (_lastKeyboardState.IsKeyUp(Keys.F11) && keyboardState.IsKeyDown(Keys.F11))
            {
                graphics.IsFullScreen = true;
                
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                graphics.ApplyChanges();
            }

            _lastKeyboardState = keyboardState;
            _chunkManager.Update(_cameraPosition);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Lerp(Color.MidnightBlue * 0.3f, Color.DodgerBlue, _sunLuminosity));
            Vector3 cameraPositionFront = _cameraPosition + _cameraDirection;
            Matrix view = Matrix.CreateLookAt(_cameraPosition, cameraPositionFront, new Vector3(0, 1, 0));

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), (float)graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight, 0.1f, 1000f);

            BoundingFrustum boundingFrustum = new BoundingFrustum(view * projection);

            _terrainEffect.Parameters["SunOrientation"].SetValue(new Vector3(0.333f, 0.533f, -0.133f));
            _terrainEffect.Parameters["SunLightColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f) * _sunLuminosity);
            _terrainEffect.Parameters["ArtificialLightColor"].SetValue(new Vector4(0.5f, 0.5f, 1.0f, 1.0f));
            _terrainEffect.Parameters["AmbiantColor"].SetValue(Color.Gray.ToVector4());
            _terrainEffect.Parameters["Texture1"].SetValue(_terrainTexture);
            _terrainEffect.Parameters["CameraPos"].SetValue(_cameraPosition);
            _terrainEffect.Parameters["FogStart"].SetValue(32.0f * 10);
            _terrainEffect.Parameters["FogEnd"].SetValue(32.0f * 20);
            _terrainEffect.Parameters["FogColor"].SetValue(Color.CornflowerBlue.ToVector4());

            GraphicsDevice.BlendState = BlendState.Opaque;

            foreach (var chunk in _chunkManager.Chunks)
            {
                if(chunk.OpaqueFaceCount == 0)
                {
                    continue;
                }

                if(!boundingFrustum.Intersects(chunk.BoundingBox))
                {
                    continue;
                }

                _terrainEffect.Parameters["World"].SetValue(chunk.Transform);
                _terrainEffect.Parameters["WorldViewProjection"].SetValue(chunk.Transform * view * projection);

                GraphicsDevice.SetVertexBuffer(chunk.VertexBufferOpaque);
                GraphicsDevice.Indices = chunk.IndexBufferOpaque;

                foreach (EffectPass pass in _terrainEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.OpaqueFaceCount);
                }
            }

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            _waterEffect.Parameters["SunOrientation"].SetValue(new Vector3(0.333f, 0.533f, -0.133f));
            _waterEffect.Parameters["SunLightColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f) * _sunLuminosity);
            _waterEffect.Parameters["ArtificialLightColor"].SetValue(new Vector4(0.5f, 0.5f, 1.0f, 1.0f));
            _waterEffect.Parameters["AmbiantColor"].SetValue(Color.Gray.ToVector4());
            _waterEffect.Parameters["Texture1"].SetValue(_terrainTexture);
            _waterEffect.Parameters["CameraPos"].SetValue(_cameraPosition);
            _waterEffect.Parameters["FogStart"].SetValue(32.0f * 10);
            _waterEffect.Parameters["FogEnd"].SetValue(32.0f * 20);
            _waterEffect.Parameters["FogColor"].SetValue(Color.CornflowerBlue.ToVector4());
            _waterEffect.Parameters["Opacity"].SetValue(1.0f);
            _waterEffect.Parameters["Opacity"].SetValue(0.3f);

            foreach (var chunk in _chunkManager.Chunks)
            {
                if (chunk.WaterFaceCount == 0)
                {
                    continue;
                }

                if (!boundingFrustum.Intersects(chunk.BoundingBox))
                {
                    continue;
                }

                _waterEffect.Parameters["World"].SetValue(chunk.Transform);
                _waterEffect.Parameters["WorldViewProjection"].SetValue(chunk.Transform * view * projection);

                GraphicsDevice.SetVertexBuffer(chunk.VertexBufferWater);
                GraphicsDevice.Indices = chunk.IndexBufferWater;

                foreach (EffectPass pass in _waterEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.WaterFaceCount);
                }
            }

            base.Draw(gameTime);
        }
    }
}
