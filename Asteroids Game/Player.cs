using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    public class Player
    {
        public int index;
        public bool win = false;
        public int life = 5;
        public int score;

        public string Name;
        public Model shipModel;
        public Matrix[] shipTransforms;
        public Vector3 color;
        public Input[] controls;
        public bool isLAN;

        public Ship ship = new Ship();
        public Bomb bomb = new Bomb();
        public PowerUp powerUp = new PowerUp();
        public Bullet[] bulletList = new Bullet[GameConstants.NumBullets];

        public Cue engineSound = null;

        public Player(int i, string n, Model sm, Matrix[] st, Vector3 clr, Input[] ctrl)
        {
            index = i;
            Name = n;
            shipModel = sm;
            shipTransforms = st;
            color = clr;
            controls = ctrl;
            isLAN = false;
        }

        public Player(int i, string n, Model sm, Matrix[] st, Vector3 clr)
        {
            index = i;
            Name = n;
            shipModel = sm;
            shipTransforms = st;
            color = clr;
            isLAN = true;
        }

        public void Update(GameTime gameTime)
        {
            // Update the ship.
            ship.Update();

            // Update the Bullets Positions.
            for (int i = 0; i < GameConstants.NumBullets; i++)
                if (bulletList[i].isActive)
                    bulletList[i].Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Update the bomb explosion.
            if (bomb.isActive)
                bomb.Update(gameTime);

            // Update the powerUp.
            if (powerUp.isActive && !WinningScreen.isActive)
                powerUp.Update(gameTime, ship.Position);

            // Get some input.
            if (!WinningScreen.isActive)
                UpdateInput();

            // Bullet-Asteroid collision check
            CheckForBulletAsteroidCollision(GamePlay.bulletModel.Meshes[0].BoundingSphere.Radius,
                GamePlay.asteroidModel.Meshes[0].BoundingSphere.Radius);

            // Ship-Asteroid collision check.
            if (ship.timer > 2)
                CheckForShipAsteroidCollision(shipModel.Meshes[0].BoundingSphere.Radius,
                    GamePlay.asteroidModel.Meshes[0].BoundingSphere.Radius);

            // Update Winning Status.
            win = true;
            for (int i = 0; i < GamePlay.playerList.Length; i++)
                if (i != index && score < GamePlay.playerList[i].score)
                    win = false;
        }

        public void UpdateInput()
        {
            // Set some audio based on whether we're pressing up or down.
            if (controls[0].getValue() != 0 || controls[1].getValue() != 0)
            {
                if (engineSound == null && ship.isActive)
                {
                    engineSound = MainClass.soundBank.GetCue("engine_2");
                    engineSound.Play();
                }
                else if (engineSound.IsPaused && ship.isActive)
                    engineSound.Resume();
                ship.Velocity += ship.RotationMatrix.Forward * ((controls[0].getValue() - controls[1].getValue()) * 8);
            }
            else
            {
                if (engineSound != null && engineSound.IsPlaying)
                    engineSound.Pause();
            }

            // Rotate the ship using the left and right arrow keys.
            ship.RotationAdd += (controls[2].getValue() - controls[3].getValue()) * 0.008f;

            // Are we shooting?
            if (controls[4].isPressed() && ship.isActive)
                ShootBullet();

            // In case you get lost, press "Right Alt" to warp back to the center.
            if (controls[5].isPressed() && life > 0)
                WarpToCenter();

            // Deploy Bomb.
            if (controls[6].isPressed() && bomb.isAvailable && ship.isActive)
            {
                MainClass.soundBank.PlayCue("bomb_deploy");
                bomb.Deploy(ship.Position + ship.Velocity * 20, index);
                if (ship.timer < 2)
                    ship.timer = 1.8;
            }
        }

        public void WarpToCenter()
        {
            ship.Position = Vector3.Zero;
            ship.Velocity = Vector3.Zero;
            ship.Rotation = 0.0f;
            ship.RotationAdd = 0;
            ship.timer = 0;

            // Make a sound when we warp.
            if (!WinningScreen.isActive)
                MainClass.soundBank.PlayCue("hyperspace_activate");

            if (ship.isActive && !WinningScreen.isActive)
                score -= GameConstants.WarpPenalty;

            ship.isActive = true;
        }

        public void ShootBullet()
        {
            //add another bullet.  Find an inactive bullet slot and use it
            //if all bullets slots are used, ignore the user input
            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                if (!bulletList[i].isActive
                    && ship.Position.X > -GameConstants.PlayfieldSizeX
                    && ship.Position.X < GameConstants.PlayfieldSizeX
                    && ship.Position.Y > -GameConstants.PlayfieldSizeY
                    && ship.Position.Y < GameConstants.PlayfieldSizeY)
                {
                    bulletList[i].velocityAdd = ship.Velocity;
                    bulletList[i].direction = ship.RotationMatrix.Forward;
                    bulletList[i].speed = GameConstants.BulletSpeedAdjustment;
                    bulletList[i].position = ship.Position + (1000 * bulletList[i].direction);
                    bulletList[i].isActive = true;
                    bulletList[i].startTimer = true;
                    MainClass.soundBank.PlayCue("tx0_fire1");
                    if (!WinningScreen.isActive)
                        score -= GameConstants.ShotPenalty;
                    if (ship.timer < 2)
                        ship.timer = 2;
                    return; //exit the loop     
                }
            }
        }

        public void CheckForBulletAsteroidCollision(float bulletRadius, float asteroidRadius)
        {
            for (int i = 0; i < GamePlay.asteroidList.Length; i++)
            {
                if (GamePlay.asteroidList[i].isActive)
                {
                    BoundingSphere asteroidSphere = new BoundingSphere(GamePlay.asteroidList[i].position, asteroidRadius * 0.95f);
                    for (int j = 0; j < bulletList.Length; j++)
                    {
                        if (bulletList[j].isActive)
                        {
                            BoundingSphere bulletSphere = new BoundingSphere(bulletList[j].position, bulletRadius);
                            if (asteroidSphere.Intersects(bulletSphere))
                            {
                                MainClass.soundBank.PlayCue("explosion2");
                                GamePlay.asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                bulletList[j].startTimer = false;
                                bulletList[j].timer = 0;
                                score += GameConstants.KillBonus;
                                if (GamePlay.random.Next(4) == 0 && !powerUp.isActive)
                                    powerUp.Initialize(GamePlay.asteroidList[i].position);
                                return; //no need to check other bullets
                            }
                        }
                    }
                }
            }
        }

        public void CheckForShipAsteroidCollision(float shipRadius, float asteroidRadius)
        {
            if (ship.isActive)
            {
                BoundingSphere shipSphere = new BoundingSphere(ship.Position, shipRadius * 0.5f);
                for (int i = 0; i < GamePlay.asteroidList.Length; i++)
                {
                    if (GamePlay.asteroidList[i].isActive)
                    {
                        BoundingSphere asteroidSphere = new BoundingSphere(GamePlay.asteroidList[i].position,
                            asteroidRadius * 0.95f);
                        if (asteroidSphere.Intersects(shipSphere))
                        {
                            //blow up ship
                            MainClass.soundBank.PlayCue("explosion3");
                            ship.isActive = false;
                            life -= 1;
                            GamePlay.asteroidList[i].isActive = false;
                            return; //exit the loop
                        }
                    }
                }
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            // Draws the bomb's explosion.
            if (bomb.isActive)
                bomb.Draw();

            // Draws the PowerUp.
            if (powerUp.isActive && !WinningScreen.isActive)
                powerUp.Draw(color);

            // Draws the ship.
            ((BasicEffect)shipModel.Meshes[0].Effects[0]).DiffuseColor = color;
            Matrix shipTransformMatrix = ship.RotationMatrix * Matrix.CreateTranslation(ship.Position);
            if (ship.isActive && (ship.timer % 0.25 < 0.16 || ship.timer > 2))
                GamePlay.DrawModel(shipModel, shipTransformMatrix, shipTransforms);

            // Draws the bullets.
            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                Matrix bulletTransform = Matrix.CreateTranslation(bulletList[i].position);
                if (bulletList[i].isActive)
                    GamePlay.DrawModel(GamePlay.bulletModel, bulletTransform, GamePlay.bulletTransforms);
            }
        }
    }
}
