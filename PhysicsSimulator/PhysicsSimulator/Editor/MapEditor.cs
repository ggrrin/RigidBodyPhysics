using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.Editor.Menu;
using PhysicsSimulator.Engine;
using PhysicsSimulator.Engine.Game;
using PhysicsSimulator.Engine.Physics;
using PhysicsSimulator.GUI;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;
using System.Collections.Generic;


namespace PhysicsSimulator.Editor
{
    /// <summary>
    /// Třída realizujici Editor map
    /// </summary>
    class MapEditor : PhysicCore
    {
        /// <summary>
        /// urcuje zda je v editoru aktivny nejaky vyskakovaci dialog
        /// </summary>
        public static bool noDialog = true;

        /// <summary>
        /// urcuje vychozi barvu obektu
        /// </summary>
        Color initialBodyColor = Color.Red;

        /// <summary>
        /// urcuje vychozi hustotu objektu
        /// </summary>
        const float initialBodyDensity = 9;


        /// <summary>
        /// kopie soucasne mapy v hernim modu
        /// </summary>
        private GameMap map;
        /// <summary>
        /// kopie soucasne mapy v hernim modu
        /// </summary>
        public GameMap Map
        {
            get { return map; }
            set { map = value; }
        }


        /// <summary>
        /// seznam teles na mape
        /// </summary>
        internal List<IPhysicBody> Bodies
        {
            get { return base.bodies; }
        }

        //LEVEL

        /// <summary>
        /// Třída realizujici Editor map
        /// </summary>
        public static bool NoDialog
        {
            get { return noDialog; }
            set { noDialog = value; }
        }

        /// <summary>
        /// drawer ke kresleni pomocnych car
        /// </summary>
        VectorDrawer vdraw;

        /// <summary>
        /// urcuje zda bezi simulace
        /// </summary>
        private bool running = false;

        /// <summary>
        /// urcuje zda bezi simulace
        /// </summary>
        public bool Running
        {
            get { return running; }
            set
            {
                if (value == true && running != true)
                {
                    BackUpMap();
                    LoadMapToEditor();
                }
                else if (value == false && running != false)
                {
                    LoadMapToEditor();
                    itemPad.Enabled = false;
                }
                running = value;
            }
        }

        /// <summary>
        /// handler pro navrat do menu
        /// </summary>
        public event EventHandler GoToMainMenu;

        /// <summary>
        /// seznam evenboxu
        /// </summary>
        public List<EventBox> EventBoxes
        {
            get { return base.eventBoxes; }
        }

        /// <summary>
        /// vstupni zarzeni
        /// </summary>
        KeyboardState keyboard, previousKeyboard;


        /// <summary>
        /// aktualne vybrany nastroj
        /// </summary>
        private Tools currentMode = Tools.None;

        /// <summary>
        /// aktualne vybrany nastroj
        /// </summary>
        public Tools CurrentMode
        {
            get { return currentMode; }
            set
            {
                currentMode = value;
                if (value != Tools.None)
                    bar.simulate.Enabled = false;
                else
                    bar.simulate.Enabled = true;
            }
        }

        /// <summary>
        /// vypis informaci
        /// </summary>
        internal Label l;

        /// <summary>
        /// horni toolbar
        /// </summary>
        internal EditorMainToolBar bar;

        /// <summary>
        /// toolbar
        /// </summary>
        internal EditorToolBar toolbar;

        /// <summary>
        /// infomacni panle o objektu
        /// </summary>
        internal EditorItemPad itemPad;

        /// <summary>
        /// kreslici platno
        /// </summary>
        internal EditorCanvas canvas;


        /// <summary>
        /// seznam bodu s kterymi se pracuje
        /// </summary>
        private Stack<Vector2> workingPoints;

        /// <summary>
        /// vychozi rychlost posouvani platna
        /// </summary>
        private const float CANVAS_MOVEMENT_SPEED = 0.6f;

        /// <summary>
        /// vychozi rychlost scalovani patna
        /// </summary>
        private const float CANVAS_SCALE_SPEED = 1.1f;

        /// <summary>
        /// inicializuje editor
        /// </summary>
        public MapEditor()
        {




            vdraw = new VectorDrawer();

            l = new Label();
            l.Location = new Vector2(500, 100);
            l.TextSize = 50;
            l.Size = new Vector2(1620, 50);


            workingPoints = new Stack<Vector2>();
            toolbar = new EditorToolBar(this);
            toolbar.OnToolSelected += toolbar_OnToolSelected;

            canvas = new EditorCanvas();
            canvas.OnClick += canvas_OnClick;

            itemPad = new EditorRigidBodyPad(this);
            itemPad.OnToolSelected += toolbar_OnToolSelected;

            bar = new EditorMainToolBar(this);


            CleanMap();
        }

        #region Tool Lifecycle

        /// <summary>
        /// provede inicializacni akce pri vybrani toolu
        /// </summary>
        /// <param name="sendr"></param>
        /// <param name="args"></param>
        void toolbar_OnToolSelected(object sendr, ToolsSelectedEventArgs args)
        {
            workingPoints = new Stack<Vector2>();
            CurrentMode = args.tool;
            this.itemPad.Enabled = false;
            l.Text = "Entrem potvrdite akci. (Esc pro zruseni akce) ";
            switch (CurrentMode)
            {
                case Tools.Selector:
                    l.Text = "Kliknutim vyberte objekt.";
                    break;
                case Tools.VelocityDrawer:
                    this.itemPad.Enabled = true;
                    workingPoints.Push((this.itemPad as EditorRigidBodyPad).Item.Position + (this.itemPad as EditorRigidBodyPad).Item.Velocity);
                    l.Text += "Dejte bod.";
                    break;
                case Tools.Box:
                    l.Text += "Dejte 2 body.";
                    break;
                case Tools.Positioner:
                    this.itemPad.Enabled = true;
                    break;
                case Tools.Rotator:
                    l.Text += "Dejte 2 body.";
                    this.itemPad.Enabled = true;
                    break;
                case Tools.Cloner:
                    l.Text += "Vyberte pozici klona";
                    break;
                case Tools.Elipse:
                    l.Text += "Dej 2 body urcujici obdelnik opsany elipse";
                    break;
                case Tools.LocationPicker:
                    l.Text += "vyberte cil teleportu";
                    workingPoints.Push(((this.itemPad as EditorItemEventBox).Box as TeleportEventBox).Location);
                    break;
            }
        }

        /// <summary>
        /// provede akce pro dany tool pri kliknu na platnlo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void canvas_OnClick(object sender, EventArgs e)
        {
            if (MapEditor.NoDialog)
            {
                switch (CurrentMode)
                {
                    case Tools.Selector:

                        foreach (var i in base.bodies)
                            if (i.Intersects(MousePosition()))
                            {
                                var j = new EditorRigidBodyPad(this);
                                j.OnToolSelected += toolbar_OnToolSelected;
                                j.Item = i;
                                itemPad = j;
                                return;
                            }

                        foreach (var i in base.eventBoxes)
                            if (i.Intersects(MousePosition()))
                            {
                                var j = new EditorItemEventBox(this);
                                j.OnToolSelected += this.toolbar_OnToolSelected;
                                j.delete.Enabled = true;
                                j.Box = i;
                                j.SetupFields();
                                itemPad = j;
                                return;
                            }

                        itemPad.Enabled = false;

                        break;
                    case Tools.Convex:
                        if (workingPoints.Count == 0)
                            this.itemPad.Enabled = false;
                        workingPoints.Push(MousePosition());
                        break;
                    case Tools.VelocityDrawer:
                        if (workingPoints.Count == 1)
                            workingPoints.Pop();
                        workingPoints.Push(MousePosition());
                        break;
                    case Tools.Box:
                        if (workingPoints.Count < 2)
                        {
                            workingPoints.Push(MousePosition());
                            l.Text = "";
                        }
                        else
                            l.Text = "Jsou treba prave 2 body.";
                        break;
                    case Tools.Rotator:
                        if (workingPoints.Count < 2)
                        {
                            workingPoints.Push(MousePosition());
                            l.Text = "";
                        }
                        else
                            l.Text = "Nastroj podporuje pouze 2 bod. (Mazani => backspace)";
                        break;

                    case Tools.Cloner:
                        if (workingPoints.Count != 1)
                            workingPoints.Push(MousePosition());
                        else if (workingPoints.Count == 1)
                        {
                            workingPoints.Pop();
                            workingPoints.Push(MousePosition());
                        }
                        break;
                    case Tools.Elipse:
                        if (workingPoints.Count < 2)
                        {
                            workingPoints.Push(MousePosition());
                            l.Text = "";
                        }
                        else
                            l.Text = "Jsou treba prave 2 body.";
                        break;
                    case Tools.LocationPicker:
                        if (workingPoints.Count != 1)
                            workingPoints.Push(MousePosition());
                        else if (workingPoints.Count == 1)
                        {
                            workingPoints.Pop();
                            workingPoints.Push(MousePosition());
                        }
                        break;

                }
            }
        }


        /// <summary>
        /// provede akce daneho toolu ktere je treba delat neustale
        /// </summary>
        private void HandelTools()
        {
            Vector2[] a = workingPoints.ToArray();
            EditorRigidBodyPad itemPad;
            switch (CurrentMode)
            {
                case Tools.VelocityDrawer:
                    itemPad = this.itemPad as EditorRigidBodyPad;
                    Vector2 asr;
                    if (workingPoints.Count == 1)
                    {
                        vdraw.Add(itemPad.Item.Position, (asr = workingPoints.Pop()) - itemPad.Item.Position, Color.Blue);
                        workingPoints.Push(asr);
                    }
                    vdraw.Add(itemPad.Item.Position, 0.3f, 0, Color.Red);

                    break;
                case Tools.Positioner:
                    itemPad = this.itemPad as EditorRigidBodyPad;
                    vdraw.Add(itemPad.Item.Position, MousePosition() - itemPad.Item.Position, Color.Green);
                    break;

                case Tools.Rotator:
                    itemPad = this.itemPad as EditorRigidBodyPad;
                    foreach (var i in workingPoints.ToArray())
                    {
                        vdraw.Add(itemPad.Item.Position, i - itemPad.Item.Position, Color.Orange);
                    }
                    vdraw.Add(itemPad.Item.Position, 0.3f, 0, Color.Red);

                    break;
            }
        }


        /// <summary>
        /// provede zaknceni pouzivani tool potvrzeni nebo zruseni toolu
        /// </summary>
        private void FinishModeOperation()
        {
            if (KeyPressed(Keys.Enter))
            {
                Vector2[] points = workingPoints.ToArray();

                switch (CurrentMode)
                {
                    case Tools.Convex:
                        ConvexBody o;
                        try
                        {
                            o = new ConvexBody(initialBodyColor, Vector2.Zero, initialBodyDensity, 1, Vector2.Zero, 0, 0, points);
                        }

                        catch (Exception e)
                        {
                            l.Text = e.Message + " (Mazani => backspace)";
                            return;
                        }

                        o.Translate(points[0] - o.PolygonLinks[0].Point1);
                        base.bodies.Add(o);

                        this.itemPad = new EditorRigidBodyPad(this);
                        this.itemPad.OnToolSelected += toolbar_OnToolSelected;
                        (this.itemPad as EditorRigidBodyPad).Item = o;
                        workingPoints.Clear();
                        break;

                    case Tools.VelocityDrawer:
                        if (workingPoints.Count == 1)
                        {
                            (itemPad as EditorRigidBodyPad).Item.Velocity = workingPoints.Pop() - (itemPad as EditorRigidBodyPad).Item.Position;
                            itemPad.SetupFields();
                            CurrentMode = Tools.None;
                        }
                        else
                            l.Text = "Udejte vektor!";

                        break;

                    case Tools.Box:
                        if (workingPoints.Count == 2)
                        {
                            this.itemPad = new EditorItemEventBox(this);
                            this.itemPad.OnToolSelected += toolbar_OnToolSelected;
                            var pad = itemPad as EditorItemEventBox;
                            pad.Save(workingPoints.Pop(), workingPoints.Pop());
                        }
                        else
                            l.Text = "Jsou treba 2 body!";
                        break;
                    case Tools.Positioner:
                        (this.itemPad as EditorRigidBodyPad).Item.Translate(MousePosition() - (this.itemPad as EditorRigidBodyPad).Item.Position);
                        break;
                    case Tools.Rotator:
                        if (workingPoints.Count == 2)
                        {
                            Vector2 itemPos = (this.itemPad as EditorRigidBodyPad).Item.Position;
                            Vector2[] a = workingPoints.ToArray();
                            Vector2 v1 = Vector2.Normalize(a[0] - itemPos);
                            Vector2 v2 = Vector2.Normalize(a[1] - itemPos);
                            (this.itemPad as EditorRigidBodyPad).Item.Rotate(itemPos, (float)(Math.Acos((float)Math.Abs(Vector2.Dot(v1, v2)))));
                            workingPoints.Clear();
                        }
                        else
                            l.Text = "Jsou treba 2 body!";
                        break;

                    case Tools.Cloner:
                        if (workingPoints.Count == 1)
                        {
                            var c = (itemPad as EditorRigidBodyPad).Item.Clone();
                            base.bodies.Add(c);
                            c.Translate(workingPoints.Pop() - (itemPad as EditorRigidBodyPad).Item.Position);
                            workingPoints.Clear();
                        }
                        else
                            l.Text = "Nejprve vyberte pozici klona.";
                        break;

                    case Tools.Elipse:
                        if (workingPoints.Count == 2)
                        {
                            this.itemPad = new EditorRigidBodyPad(this);
                            this.itemPad.OnToolSelected += toolbar_OnToolSelected;
                            var pad = itemPad as EditorRigidBodyPad;
                            Vector2[] a = workingPoints.ToArray();

                            var q = new ElipseBody(initialBodyColor, Vector2.Zero, initialBodyDensity, 1, Vector2.Zero, 0, (float)Math.Abs(a[0].X - a[1].X) / 2, (float)Math.Abs(a[0].Y - a[1].Y) / 2, 0);
                            q.Translate(a[0] + 0.5f * (a[1] - a[0]));
                            pad.Item = q;
                            base.bodies.Add(q);

                            workingPoints.Clear();
                        }
                        else
                            l.Text = "Jsou treba 2 body!";
                        break;
                    case Tools.LocationPicker:
                        ((this.itemPad as EditorItemEventBox).Box as TeleportEventBox).Location = workingPoints.Pop();
                        currentMode = Tools.None;
                        break;
                }


            }
            else if (KeyPressed(Keys.Escape))
            {
                switch (CurrentMode)
                {
                    case Tools.Selector:
                        this.itemPad.Enabled = false;
                        break;

                    case Tools.Box:
                        this.itemPad = new EditorItemEventBox(this);
                        this.itemPad.Enabled = false;
                        break;
                }

                workingPoints.Clear();
                CurrentMode = Tools.None;
            }
        }

        #endregion

        #region


        /// <summary>
        /// pro opusteni editoru
        /// </summary>
        internal void LeaveEditor()
        {
            if (GoToMainMenu != null)
                GoToMainMenu(this, new EventArgs());
        }


        /// <summary>
        /// vycisti vsechny objekty na mape
        /// </summary>
        internal void CleanMap()
        {
            //clean map
            map = new GameMap();
            //implies clen editor :)))
            LoadMapToEditor();
        }

        /// <summary>
        /// nahraje mapu do editoru
        /// </summary>
        internal void LoadMapToEditor()
        {
            base.unitOnWidth = map.unitsOnWidth;
            this.bodies = new List<IPhysicBody>();
            foreach (var i in map.Bodies)
                base.bodies.Add(i.Clone());

            base.eventBoxes = new List<EventBox>();
            foreach (var i in map.EventBoxes)
                this.EventBoxes.Add(i.Clone());
        }


        /// <summary>
        /// ulozi mapu z editoru do dat
        /// </summary>
        internal void BackUpMap()
        {
            map = new GameMap();
            map.unitsOnWidth = base.unitOnWidth;
            foreach (var i in bodies)
                map.Bodies.Add(i.Clone());

            foreach (var i in eventBoxes)
                map.EventBoxes.Add(i.Clone());
        }

        /// <summary>
        /// odstarni dane teleso z mapu
        /// </summary>
        /// <param name="body"></param>
        internal void RemoveBody(IPhysicBody body)
        {
            base.bodies.Remove(body);
        }

        /// <summary>
        /// urcuje zda bylo kliknuto na danou klavesu
        /// </summary>
        /// <param name="key">klavesa k otestovani</param>
        /// <returns></returns>
        private bool KeyPressed(Keys key)
        {
            return keyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }


        /// <summary>
        /// aktualizuje pozici platna 
        /// </summary>
        protected override void UpdateCanvasPositionLocation()
        {
            if (MapEditor.NoDialog)
            {
                if (keyboard.IsKeyDown(Keys.Up))
                    cameraPosition.Y += CANVAS_MOVEMENT_SPEED;
                else if (keyboard.IsKeyDown(Keys.Down))
                    cameraPosition.Y -= CANVAS_MOVEMENT_SPEED;

                if (keyboard.IsKeyDown(Keys.Left))
                    cameraPosition.X -= CANVAS_MOVEMENT_SPEED;
                else if (keyboard.IsKeyDown(Keys.Right))
                    cameraPosition.X += CANVAS_MOVEMENT_SPEED;

                if (keyboard.IsKeyDown(Keys.OemPlus))
                    unitOnWidth /= CANVAS_SCALE_SPEED;
                else if (keyboard.IsKeyDown(Keys.OemMinus))
                    unitOnWidth *= CANVAS_SCALE_SPEED;
            }

            base.UpdateCanvasPositionLocation();
        }

        /// <summary>
        /// inicializuje pracovni body pro vykresleni
        /// </summary>
        private void AddPointsToVectorDrawer()
        {

            if (workingPoints != null)
                foreach (var i in workingPoints)
                {
                    vdraw.Add(i, 0.3f, 0.3f, Color.Red);
                }
        }

        /// <summary>
        /// vykresli pocatek
        /// </summary>

        private void DrawOrign()
        {
            vdraw.Add(Vector2.Zero, 10, 0, Color.Green);
            vdraw.Add(new Vector2(1, 1), 10, 0, Color.Green);


        }


        /// <summary>
        /// aktualizuje vse
        /// </summary>
        /// <param name="time"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            vdraw.Clear();
            keyboard = Keyboard.GetState();

            PointsUndo();

            HandelTools();

            FinishModeOperation();


            this.UpdateCanvasPositionLocation();

            if (Running)
                base.Update(time);

            toolbar.Update(time, Vector2.Zero);
            canvas.Update(time, Vector2.Zero);
            itemPad.Update(time, Vector2.Zero);
            bar.Update(time, Vector2.Zero);

            DrawOrign();
            AddPointsToVectorDrawer();
            l.Update(time, Vector2.Zero);


            previousKeyboard = keyboard;
        }

        /// <summary>
        /// vymaze posledni pracovni bod
        /// </summary>
        private void PointsUndo()
        {
            if (keyboard.IsKeyDown(Keys.Back) && previousKeyboard.IsKeyUp(Keys.Back))
                if (workingPoints != null && workingPoints.Count > 0)
                    workingPoints.Pop();
        }


        /// <summary>
        /// vykresli mapu
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        public override void Draw(SpriteBatch sprite, GameTime time)
        {
            base.Draw(sprite, time);
            vdraw.Draw();
        }


        /// <summary>
        /// vykresli GUI
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        public void DrawUI(SpriteBatch sprite, GameTime time)
        {
            toolbar.Draw(sprite, time, Vector2.Zero);
            canvas.Draw(sprite, time, Vector2.Zero);
            itemPad.Draw(sprite, time, Vector2.Zero);
            l.Draw(sprite, time, Vector2.Zero);
            bar.Draw(sprite, time, Vector2.Zero);
        }

        #endregion

    }
}
