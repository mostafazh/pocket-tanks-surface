using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Pocket_Tanks_Surface
{
    /// <summary>
    /// This is the main type for your application.
    /// </summary>
    public class App1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private ContactTarget contactTarget;
        private UserOrientation currentOrientation = UserOrientation.Bottom;
        private Color backgroundColor = new Color(81, 81, 81);
        private bool applicationLoadCompleteSignalled;
        private Matrix screenTransform = Matrix.Identity;

        private SpriteBatch spriteBatch;
        private Background background;

        // application state: Activated, Previewed, Deactivated,
        // start in Activated state
        private bool isApplicationActivated = true;
        private bool isApplicationPreviewed;

        /// <summary>
        /// The graphics device manager for the application.
        /// </summary>
        protected GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        /// <summary>
        /// The target receiving all surface input for the application.
        /// </summary>
        protected ContactTarget ContactTarget
        {
            get { return contactTarget; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public App1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Engine.SetGame(this);
        }

        /// <summary>
        /// Allows the app to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SetWindowOnSurface();
            InitializeSurfaceInput();
            Components.ComponentAdded+=new EventHandler<GameComponentCollectionEventArgs>(Components_ComponentAdded);
            // Set the application's orientation based on the current launcher orientation
            currentOrientation = ApplicationLauncher.Orientation;

            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;

            // Setup the UI to transform if the UI is rotated.
            if (currentOrientation == UserOrientation.Top)
            {
                // Create a rotation matrix to orient the screen so it is viewed correctly when the user orientation is 180 degress different.
                Matrix rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(180));
                Matrix translation = Matrix.CreateTranslation(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, 0);

                screenTransform = rotation * translation;
            }
            Engine.Instance.P1.tank.Initialize();
            Engine.Instance.P2.tank.Initialize();
            base.Initialize();
        }

        private void Components_ComponentAdded(object sender, GameComponentCollectionEventArgs a)
        {
            Console.WriteLine(a);
        }

        /// <summary>
        /// Moves and sizes the window to cover the input surface.
        /// </summary>
        private void SetWindowOnSurface()
        {
            System.Diagnostics.Debug.Assert(Window.Handle != System.IntPtr.Zero,
                "Window initialization must be complete before SetWindowOnSurface is called");
            if (Window.Handle == System.IntPtr.Zero)
                return;

            // We don't want to run in full-screen mode because we need
            // overlapped windows, so instead run in windowed mode
            // and resize to take up the whole surface with no border.

            // Make sure the graphics device has the correct back buffer size.
            InteractiveSurface interactiveSurface = InteractiveSurface.DefaultInteractiveSurface;
            if (interactiveSurface != null)
            {
                graphics.PreferredBackBufferWidth = interactiveSurface.Width;
                graphics.PreferredBackBufferHeight = interactiveSurface.Height;
                graphics.ApplyChanges();

                // Remove the border and position the window.
                Program.RemoveBorder(Window.Handle);
                Program.PositionWindow(Window);
            }
        }

        /// <summary>
        /// Initializes the surface input system. This should be called after any window
        /// initialization is done, and should only be called once.
        /// </summary>
        private void InitializeSurfaceInput()
        {
            System.Diagnostics.Debug.Assert(Window.Handle != System.IntPtr.Zero,
                "Window initialization must be complete before InitializeSurfaceInput is called");
            if (Window.Handle == System.IntPtr.Zero)
                return;
            System.Diagnostics.Debug.Assert(contactTarget == null,
                "Surface input already initialized");
            if (contactTarget != null)
                return;

            // Create a target for surface input.
            contactTarget = new ContactTarget(Window.Handle, EventThreadChoice.OnBackgroundThread);
            contactTarget.EnableInput();
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: Load any content
            // Load all textures
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = new Background(Content.Load<Texture2D>("Background"), 1024, 768);
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
            background.Unload();
        }

        /// <summary>
        /// Allows the app to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (isApplicationActivated || isApplicationPreviewed)
            {
                if (isApplicationActivated)
                {
                    // TODO: Process contacts
                    // Use the following code to get the state of all current contacts.
                    // ReadOnlyContactCollection contacts = contactTarget.GetState();
                }
                ReadOnlyContactCollection rocc = contactTarget.GetState();
                if(rocc != null && rocc.Count >= 1)
                {
                    Contact c = rocc.First<Contact>();
                    contactTarget.ContactAdded += new EventHandler<ContactEventArgs>(contactTarget_ContactAdded);
                    
                }
                Engine.Instance.P1.tank.Update(gameTime);
                Engine.Instance.P2.tank.Update(gameTime);
                // TODO: Add your update logic here
            }
            base.Update(gameTime);
        }

        private void contactTarget_ContactAdded(object sender, ContactEventArgs a)
        {
            Console.WriteLine("Click");
            Contact c = a.Contact;
            Vector2 newPosition = new Vector2(c.CenterX, c.CenterY);
            if (isValidMove(newPosition))
            {
                Engine.Instance.Move(newPosition);
            }
        }

        private bool isValidMove(Vector2 newPosition)
        {
            return newPosition.Y >= 609 && newPosition.Y <= 659;
        }


        /// <summary>
        /// This is called when the app should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!applicationLoadCompleteSignalled)
            {
                // Dismiss the loading screen now that we are starting to draw
                ApplicationLauncher.SignalApplicationLoadComplete();
                applicationLoadCompleteSignalled = true;
            }

            //TODO: Rotate the UI based on the value of screenTransform here if desired
            graphics.GraphicsDevice.Clear(backgroundColor);
            //TODO: Add your drawing code here
            background.Draw(spriteBatch);
            Engine.Instance.P1.tank.Draw();
            Engine.Instance.P2.tank.Draw();
            //TODO: Avoid any expensive logic if application is neither active nor previewed
            base.Draw(gameTime);
        }

        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            // update application state
            isApplicationActivated = true;
            isApplicationPreviewed = false;

            //TODO: Enable audio, animations here

            //TODO: Optionally enable raw image here
        }

        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            // update application state
            isApplicationActivated = false;
            isApplicationPreviewed = true;

            //TODO: Disable audio here if it is enabled

            //TODO: Optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            // update application state
            isApplicationActivated = false;
            isApplicationPreviewed = false;

            //TODO: Disable audio, animations here

            //TODO: Disable raw image if it's enabled
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    IDisposable graphicsDispose = graphics as IDisposable;
                    if (graphicsDispose != null)
                    {
                        graphicsDispose.Dispose();
                        graphicsDispose = null;
                    }

                    if (contactTarget != null)
                    {
                        contactTarget.Dispose();
                        contactTarget = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}
