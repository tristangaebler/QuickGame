﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using QuickGame.Model;
using QuickGame.View;

namespace QuickGame.Controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class QuickGame : Game
	{
		Texture2D dynomiteTexture;
		List<Projectile> dynomites;
		TimeSpan dynomiteTime;
		TimeSpan previousDynomiteTime;



		Texture2D poisonTexture;
		List<Poision> enemyPoison;
		TimeSpan poisonSpawnTime;
		TimeSpan previousTime;
		Random randomPoison;

		//Number that holds the player score
		int score;
		// The font used to display UI elements
		SpriteFont font;
		Texture2D explosionTexture;
		List<Animation> explosions;
		// The sound that is played when a laser is fired
		SoundEffect laserSound;

		// The sound used when the player or an enemy dies
		SoundEffect explosionSound;

		// The music played during gameplay
		Song gameplayMusic;
		Texture2D projectileTexture;
		//Texture2D poison;
		List<Projectile> projectiles;

		// The rate of fire of the player laser
		TimeSpan fireTime;
		TimeSpan previousFireTime;
		// Enemies
		Texture2D enemyTexture;
		List<Enemy> enemies;

		// The rate at which the enemies appear
		TimeSpan enemySpawnTime;
		TimeSpan previousSpawnTime;

		// A random number generator
		Random random;
		// Image used to display the static background
		Texture2D mainBackground;

		// Parallaxing Layers
		Parallaxingbackground bgLayer1;
		Parallaxingbackground bgLayer2;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		// Represents the player 

		Player player;

		// Keyboard states used to determine key presses
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;

		// Gamepad states used to determine button presses
		GamePadState currentGamePadState;
		GamePadState previousGamePadState; 

		// A movement speed for the player
		float playerMoveSpeed;

		public QuickGame ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			player = new Player();

			bgLayer1 = new Parallaxingbackground();
			bgLayer2 = new Parallaxingbackground();

			projectiles = new List<Projectile>();

			//poison = new Texture2D ();

			explosions = new List<Animation>();
			//Set player's score to zero
			score = 0;
			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(.15f);

			// Initialize the enemies list
			enemies = new List<Enemy> ();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator
			random = new Random();

			enemyPoison = new List<Poision> ();
			previousTime = TimeSpan.Zero;
			poisonSpawnTime = TimeSpan.FromSeconds (1.0f);
			random = new Random ();

			dynomites = new List<Projectile> ();
			dynomiteTime = TimeSpan.FromSeconds (.15f);


			// Set a constant player move speed
			playerMoveSpeed = 8.0f;

			//Enable the FreeDrag gesture.
			TouchPanel.EnabledGestures = GestureType.FreeDrag;
			// TODO: Add your initialization logic here
            
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			// Load the player resources 
			//Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,GraphicsDevice.Viewport.TitleSafeArea.Y +GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
				+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition);

			// Load the parallaxing background
			bgLayer1.Initialize(Content, "Texture/bgLayer1", GraphicsDevice.Viewport.Width, -1);
			bgLayer2.Initialize(Content, "Texture/bgLayer2", GraphicsDevice.Viewport.Width, -2);
			enemyTexture = Content.Load<Texture2D>("Animation/mineAnimation");
		
			projectileTexture = Content.Load<Texture2D>("Texture/laser");
			poisonTexture = Content.Load<Texture2D> ("Texture/poisonBottle");
			explosionTexture = Content.Load<Texture2D>("Animation/explosion"); 
			dynomiteTexture = Content.Load<Texture2D> ("Texture/Dynomite");
			// Load the music
			gameplayMusic = Content.Load<Song>("Sound/gameMusic");

			// Load the laser and explosion sound effect
			laserSound = Content.Load<SoundEffect>("Sound/laserFire");
			explosionSound = Content.Load<SoundEffect>("Sound/explosion");
	

			// Load the score font
			font = Content.Load<SpriteFont>("Font/gameFont");
			// Start the music right away
			PlayMusic(gameplayMusic);


			mainBackground = Content.Load<Texture2D>("Texture/mainbackground");
			//TODO: use this.Content to load your game content here 
		}

		private void AddDynomite(Vector2 position)
		{
			Dynomite dynomite = new Dynomite(); 
			dynomite.Initialize(GraphicsDevice.Viewport, dynomiteTexture,position); 
			//dynomites.Add(dynomite);
		}


		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { }
		}


		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif
            
			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);
			// Update the parallaxing background
		

			//Update the player
			UpdatePlayer(gameTime);
			bgLayer1.Update();
			bgLayer2.Update();
			UpdateEnemies(gameTime);
			UpdatePoison (gameTime);

			// Update the collision
			UpdateCollision();
			// Update the projectiles
			UpdateProjectiles();
			UpdateDynomite();

			// Update the explosions
			UpdateExplosions(gameTime);
            
			base.Update (gameTime);
		}

		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile(); 
			projectile.Initialize(GraphicsDevice.Viewport, projectileTexture,position); 
			projectiles.Add(projectile);
		}

		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update(gameTime);

			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X *playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y *playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) ||
				currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) ||
				currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) ||
				currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) ||
				currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}

			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, 0,GraphicsDevice.Viewport.Width - player.Width);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, 0,GraphicsDevice.Viewport.Height - player.Height);

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
				laserSound.Play();
			}

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddDynomite(player.Position + new Vector2(player.Width / 2, 0));
			}
			// reset score if player health goes to zero
			if (player.Health <= 0)
			{
				player.Health = 100;
				score = 0;
			}
		}

		private void UpdateDynomite()
		{
			// Update the Projectiles
			for (int i = dynomites.Count - 1; i >= 0; i--) 
			{
				dynomites[i].Update();

				if (projectiles[i].Active == false)
				{
					dynomites.RemoveAt(i);
				} 
			}
		}

		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--) 
			{
				projectiles[i].Update();

				if (projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				} 
			}
		}
		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);

			// Start drawing
			spriteBatch.Begin();

			spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);

			// Draw the Enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch);
				// Draw the Projectiles
			}
			for (int i = 0; i < enemyPoison.Count; i++)
			{
				enemyPoison[i].Draw(spriteBatch);
				// Draw the Projectiles
			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Draw(spriteBatch);
			}
			// Draw the Projectiles
			for (int i = 0; i < dynomites.Count; i++)
			{
				dynomites[i].Draw(spriteBatch);
			}
			// Draw the explosions
			for (int i = 0; i < explosions.Count; i++)
			{
				explosions[i].Draw(spriteBatch);
			}
			// Draw the score
			spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
			// Draw the player health
			spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
			// Draw the Player
			player.Draw(spriteBatch);

			// Stop drawing
			spriteBatch.End();
            

			base.Draw (gameTime);


		}

		private void AddEnemy()
		{ 
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30,Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation, position); 

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) 
			{

				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddEnemy();
			}

			// Update the Enemies
			for (int i = enemies.Count - 1; i >= 0; i--) 
			{
				enemies[i].Update(gameTime);

				if (enemies[i].Active == false)
				{
					// If not active and health <= 0
					if (enemies[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(enemies[i].Position);
						//Add to the player's score
						score += enemies[i].Value;
						explosionSound.Play();
					}
					enemies.RemoveAt(i);
				} 
			}
		}

		private void UpdateExplosions(GameTime gameTime)
		{
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if (explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}


		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture,position, 134, 134, 12, 45, Color.White, 1f,false);
			explosions.Add(explosion);
		}

		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X,
				(int)player.Position.Y,
				player.Width,
				player.Height);

			// Do the collision between the player and the enemies
			for (int i = 0; i <enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X,
					(int)enemies[i].Position.Y,
					enemies[i].Width,
					enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false; 
				}

			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - 
						projectiles[i].Width / 2,(int)projectiles[i].Position.Y - 
						projectiles[i].Height / 2,projectiles[i].Width, projectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
						(int)enemies[j].Position.Y - enemies[j].Height / 2,
						enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}

			// Projectile vs Enemy Collision
			for (int i = 0; i < dynomites.Count; i++) {
				for (int j = 0; j < enemies.Count; j++) {
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle ((int)dynomites [i].Position.X -
					dynomites [i].Width / 2, (int)dynomites [i].Position.Y -
					dynomites [i].Height / 2, dynomites [i].Width, dynomites [i].Height);

					rectangle2 = new Rectangle ((int)enemies [j].Position.X - enemies [j].Width / 2,
						(int)enemies [j].Position.Y - enemies [j].Height / 2,
						enemies [j].Width, enemies [j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects (rectangle2)) {
						enemies [j].Health -= projectiles [i].Damage;
						dynomites [i].Active = false;
					}
				}
			}
		}

		private void AddPoison()
		{
			Animation poisonAnimation = new Animation ();

			poisonAnimation.Initialize (poisonTexture, Vector2.Zero, 80, 35, 15, 30, Color.White, 1f, true);

			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +poisonTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

			Poision poison = new Poision ();

			poison.Initialize (poisonAnimation, position);

			enemyPoison.Add (poison);
		}

		private void UpdatePoison(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) 
			{
				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddPoison();
			}

			// Update the Enemies
			for (int i = enemyPoison.Count - 1; i >= 0; i--) 
			{
				enemyPoison[i].Update(gameTime);

				if (enemyPoison[i].Active == false)
				{
					enemyPoison.RemoveAt(i);
				} 
			}
		}
	}
}

