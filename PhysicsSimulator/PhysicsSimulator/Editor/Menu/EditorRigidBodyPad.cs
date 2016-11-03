using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.Engine.Physics;
using PhysicsSimulator.GUI;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.Editor.Menu
{
    /// <summary>
    /// Třída realizujcí pad pro nastaveni vlastnosí IPhysiscbody objektů v editori.
    /// </summary>
    class EditorRigidBodyPad : EditorItemPad
    {
        const int LABEL_COUNT = 12;


        protected override int labelCount
        {
            get { return LABEL_COUNT; }
        }


        /// <summary>
        /// teleso se kterym se pracuje
        /// </summary>
        protected IPhysicBody item;

        /// <summary>
        /// teleso se kterym se pracuje
        /// </summary>
        public IPhysicBody Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
                SetupFields();
                Enabled = true;
            }
        }


        protected Button stuck;
        protected Button density;

        protected Button velocityX;
        protected Button velocityY;
        protected Button velocityLength;
        protected Button velocityDraw, positioner;

        protected Button angularVelocity;

        protected Button rotationL;
        protected Button rotationR;

        protected Button colorR;
        protected Button colorG;
        protected Button colorB;

        protected Button setBall, clone;

        /// <summary>
        /// Inicializace padu a svazani s editorum
        /// </summary>
        /// <param name="editor">dany editor</param>
        public EditorRigidBodyPad(MapEditor editor)
            : base(editor)
        { }


        /// <summary>
        /// inicializace GUI component
        /// </summary>
        protected override void InitializeComponent()
        {
            //Created new instancs

            labelsText = new string[LABEL_COUNT]{
                "density", "velX", "velY", "velLen", "draw vel", "ang vel", "positioning", "color R" ,"G","B","ovladateny", "duplikace"
                };

            InitializeLabeks();

            /////////////////////////////////////////
            stuck = new Button();
            density = new Button();
            velocityX = new Button();
            velocityY = new Button();
            velocityLength = new Button();
            velocityDraw = new Button();

            angularVelocity = new Button();
            rotationL = new Button();
            rotationR = new Button();
            positioner = new Button();
            colorR = new Button();
            colorG = new Button();
            colorB = new Button();

            setBall = new Button();

            clone = new Button();

            //stuck            
            stuck.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            stuck.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            stuck.Location = new Vector2(LABEL_MARGIN, 30);
            stuck.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            stuck.TextSize = TEXT_SIZE;
            stuck.Text = "Stuck";
            stuck.OnClick += new EventHandler(stuck_OnClick);
            Controls.Add(stuck);

            //density            
            density.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            density.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            density.Location = new Vector2(BUTTON_MARGIN, 80);
            density.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            density.TextSize = TEXT_SIZE;
            density.Text = "0";
            density.OnClick += new EventHandler(density_OnClick);
            density.TextChanged += delegate(object sender, EventArgs e)
            {
                stuck.Text = "Stuck";
            };
            Controls.Add(density);

            //velocityX            
            velocityX.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            velocityX.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            velocityX.Location = new Vector2(BUTTON_MARGIN, 130);
            velocityX.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            velocityX.TextSize = TEXT_SIZE;
            velocityX.Text = "0";
            velocityX.OnClick += new EventHandler(val_OnClick);
            velocityX.TextChanged += velocityY_TextChanged;
            Controls.Add(velocityX);

            //velocityY
            velocityY.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            velocityY.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            velocityY.Location = new Vector2(BUTTON_MARGIN, 180);
            velocityY.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            velocityY.TextSize = TEXT_SIZE;
            velocityY.Text = "0";
            velocityY.OnClick += new EventHandler(val_OnClick);
            velocityY.TextChanged += velocityY_TextChanged;
            Controls.Add(velocityY);

            //velocityLength            
            velocityLength.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            velocityLength.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            velocityLength.Location = new Vector2(BUTTON_MARGIN, 230);
            velocityLength.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            velocityLength.TextSize = TEXT_SIZE;
            velocityLength.Text = "0";
            velocityLength.OnClick += new EventHandler(valLen_OnClick);
            Controls.Add(velocityLength);

            //velocityDraw
            velocityDraw.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            velocityDraw.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            velocityDraw.Location = new Vector2(BUTTON_MARGIN, 280);
            velocityDraw.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            velocityDraw.TextSize = TEXT_SIZE - 5;
            velocityDraw.Text = "Draw vel";
            velocityDraw.OnClick += velocityDraw_OnClick;
            Controls.Add(velocityDraw);

            //positioner
            positioner.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            positioner.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            positioner.Location = new Vector2(BUTTON_MARGIN, 380);
            positioner.Size = new Vector2(BUTTON_WIDTH / 2, BUTTON_HEIGHT);
            positioner.TextSize = TEXT_SIZE;
            positioner.Text = "Pos";
            positioner.OnClick += positioner_OnClick;
            Controls.Add(positioner);

            //angularVelocity
            angularVelocity.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            angularVelocity.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            angularVelocity.Location = new Vector2(BUTTON_MARGIN, 330);
            angularVelocity.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            angularVelocity.TextSize = TEXT_SIZE;
            angularVelocity.Text = "0";
            angularVelocity.OnClick += new EventHandler(val_OnClick);
            Controls.Add(angularVelocity);

            /*//rotationL
            rotationL.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            rotationL.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            rotationL.Location = new Vector2(BUTTON_MARGIN, 380);
            rotationL.Size = new Vector2(BUTTON_WIDTH / 2, BUTTON_HEIGHT);
            rotationL.TextSize = TEXT_SIZE;
            rotationL.Text = "L";            
            Controls.Add(rotationL);*/

            //rotationR
            rotationR.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            rotationR.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            rotationR.Location = new Vector2(BUTTON_MARGIN + BUTTON_WIDTH / 2 + 10, 380);
            rotationR.Size = new Vector2(BUTTON_WIDTH / 2, BUTTON_HEIGHT);
            rotationR.TextSize = TEXT_SIZE;
            rotationR.Text = "Rot";
            rotationR.OnClick += rotator_OnClick;
            Controls.Add(rotationR);

            //colorR
            colorR.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            colorR.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            colorR.Location = new Vector2(BUTTON_MARGIN, 430);
            colorR.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            colorR.TextSize = TEXT_SIZE;
            colorR.Text = "0";
            colorR.OnClick += new EventHandler(val_OnClick);
            Controls.Add(colorR);

            //colorG
            colorG.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            colorG.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            colorG.Location = new Vector2(BUTTON_MARGIN, 480);
            colorG.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            colorG.TextSize = TEXT_SIZE;
            colorG.Text = "0";
            colorG.OnClick += new EventHandler(val_OnClick);
            Controls.Add(colorG);

            //colorB
            colorB.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            colorB.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            colorB.Location = new Vector2(BUTTON_MARGIN, 530);
            colorB.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            colorB.TextSize = TEXT_SIZE;
            colorB.Text = "0";
            colorB.OnClick += new EventHandler(val_OnClick);
            Controls.Add(colorB);

            //setBall
            setBall.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            setBall.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            setBall.Location = new Vector2(BUTTON_MARGIN, 580);
            setBall.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            setBall.TextSize = TEXT_SIZE;
            setBall.Text = "Set as Ball";
            setBall.OnClick += new EventHandler(SetBall_OnClick);
            Controls.Add(setBall);

            //clone
            clone.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            clone.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            clone.Location = new Vector2(BUTTON_MARGIN, 630);
            clone.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            clone.TextSize = TEXT_SIZE;
            clone.Text = "Clone";
            clone.OnClick += new EventHandler(clone_OnClick);
            Controls.Add(clone);
        }

        /// <summary>
        /// zobrazi vyskakovaci dialog pro napsani hustoty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void density_OnClick(object sender, EventArgs e)
        {

            var dialog = new EditorInputDialog();
            dialog.Text = "Hodnotu mezi 1 a 9";
            dialog.Exit += (object senderI, ExitEventArgs args) =>
            {
                if (args.Output != null)
                    ((Button)sender).Text = Math.Min(9, Math.Max(float.Parse(args.Output), 1)).ToString();


                this.InnerFence = null;
                UpdateItem();
            };

            this.InnerFence = dialog;

        }

        /// <summary>
        /// nastavi teleso na nehybne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stuck_OnClick(object sender, EventArgs e)
        {
            item.Density = Configuration.stuckDensity;
            stuck.Text = "Stucked";
            SetupFields();
        }

        /// <summary>
        /// vybere kolonovaci tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clone_OnClick(object sender, EventArgs e)
        {
            base.InvokeToolSelected(Tools.Cloner);
        }


        /// <summary>
        /// vebere rotovaci tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotator_OnClick(object sender, EventArgs e)
        {
            base.InvokeToolSelected(Tools.Rotator);
        }


        /// <summary>
        /// vybere posouvaci tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void positioner_OnClick(object sender, EventArgs e)
        {
            base.InvokeToolSelected(Tools.Positioner);
        }


        /// <summary>
        /// vybere teleso jako hracovo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetBall_OnClick(object sender, EventArgs e)
        {
            item.Player ^= true;
            setBall.Text = item.Player.ToString();
        }


        /// <summary>
        /// inicializuje tool pro kresleni vektoru rychlosti telesa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void velocityDraw_OnClick(object sender, EventArgs e)
        {
            base.InvokeToolSelected(Tools.VelocityDrawer);
        }


        /// <summary>
        /// vymaze teleso z mapy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void delete_OnClick(object sender, EventArgs e)
        {
            base.ClosePad();
            editor.RemoveBody(Item);
        }

        /// <summary>
        /// aktualizuje udaje o rychlsti v GUUI
        /// </summary>
        void ReloadXY()
        {
            var v = new Vector2(float.Parse(velocityX.Text), float.Parse(velocityY.Text));
            if (v != Vector2.Zero)
                v.Normalize();
            v *= float.Parse(velocityLength.Text);
            velocityX.Text = v.X.ToString();
            velocityY.Text = v.Y.ToString();
        }

        /// <summary>
        /// aktualizuje udaje o rychlosti v GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void velocityY_TextChanged(object sender, EventArgs e)
        {
            velocityLength.Text = new Vector2(float.Parse(velocityX.Text), float.Parse(velocityY.Text)).Length().ToString();
        }


        /// <summary>
        /// zapne vyskakovaci dialog pro zadani hodnot a ulozi do telesa v mape
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void val_OnClick(object sender, EventArgs e)
        {
            var dialog = new EditorInputDialog();
            dialog.Exit += (object senderI, ExitEventArgs args) =>
            {
                if (args.Output != null)
                {
                    ((Button)sender).Text = args.Output;
                    ReloadXY();
                }

                this.InnerFence = null;
                UpdateItem();
            };

            this.InnerFence = dialog;
        }


        /// <summary>
        /// zobrazi vyskakovai dialog pro zadani hodnoty a nasledne ulozi do telesa na mape
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void valLen_OnClick(object sender, EventArgs e)
        {

            var dialog = new EditorInputDialog();
            //dialog.NoZero = true;
            dialog.Exit += (object senderI, ExitEventArgs args) =>
            {
                if (args.Output != null)
                {
                    ((Button)sender).Text = args.Output;
                    if (float.Parse(args.Output) == 0)
                    {
                        item.Velocity = Vector2.Zero;
                        SetupFields();
                    }

                    ReloadXY();
                }

                this.InnerFence = null;
                UpdateItem();

            };

            this.InnerFence = dialog;
        }


        /// <summary>
        /// ulozi data z GUI do telesa
        /// </summary>
        public void UpdateItem()
        {
            Item.Density = float.Parse(density.Text);
            Item.Velocity = new Vector2(float.Parse(velocityX.Text), float.Parse(velocityY.Text));
            Item.AngularVelocity = new Vector3(0, 0, float.Parse(angularVelocity.Text));
            Item.Color = new Color((int)float.Parse(colorR.Text), (int)float.Parse(colorG.Text), (int)float.Parse(colorB.Text));
        }


        /// <summary>
        /// Aktualizuje GUI podle udaju v telese
        /// </summary>
        public override void SetupFields()
        {
            density.Text = Item.Density.ToString();
            velocityX.Text = Item.Velocity.X.ToString();
            velocityY.Text = Item.Velocity.Y.ToString();
            velocityLength.Text = Item.Velocity.Length().ToString();
            angularVelocity.Text = Item.AngularVelocity.Z.ToString();
            colorR.Text = Item.Color.R.ToString();
            colorG.Text = Item.Color.G.ToString();
            colorB.Text = Item.Color.B.ToString();
            setBall.Text = Item.Player.ToString();
        }

    }
}
