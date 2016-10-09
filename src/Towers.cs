/* Jesse Bosshardt 
 * 
 * Towers of Hanoi Animation Project: 
 *  
 * This program solves and animates the Towers of Hanoi Game (Algorithm)
 * 
 * I found myself a little bored while watching the animation, so I have added some sounds and music
 * to add more functionality to the program.
 * 
 * The music is all original music that I wrote in high school. 
 *  
 * Forms: Splash.cs, Towers.cs
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using WMPLib;


namespace TowersOfHanoi
{

    public partial class Towers : Form
    {
        
        private class Disk
        {

            public Color Color { get; set; }
            public int Width { get; set; }
            public Point Position { get; set; }
            public string Peg { get; set; }

            public Disk(Color color, int width, Point position, string peg)
            {
                Color = color;
                Width = width;
                Position = position;
                Peg = peg;
            }

        }

        private class Peg
        {
            public Stack<Disk> Disks { get; set; }
            public Point Position { get; set; }

            public Peg(Point position)
            {
                Disks = new Stack<Disk>();
                Position = position;
            }
        }
       
        private class NextMove
        {
            public Peg Source {get;set;}
            public Peg Destination { get; set; }

            public NextMove(Peg source, Peg destination) {
                Source = source;
                Destination = destination;
            }
        }

        private Peg A;
        private Peg B;
        private Peg C;
        private ArrayList moveList;
        private Color[] colors;
        private string[] sounds;
        private string[] music;
        private Thread t;
        private Disk active;
        private int speed;   
        private WindowsMediaPlayer soundPlayer;
        private WindowsMediaPlayer musicPlayer;
        private string activeSound;
        private bool paused;
        private int rings;


        public delegate void UpdateLbl();

        public Towers(int rings)
        {
            this.rings = rings;
            InitializeComponent();
            DoubleBuffered = true;
            speed = 4;       
            moveList = new ArrayList();            
            colors = new Color[] {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Fuchsia, Color.Purple, Color.Teal};           
            sounds = new string[] { "sounds/cat.wav", "sounds/burp.wav", "sounds/lazer.wav", "sounds/click.wav" };
            music = new string[] { "sounds/NoReturn.mp3", "sounds/MindBound.mp3", "sounds/ALongDay.mp3", "sounds/Clairvoyance.mp3" };
            soundPlayer = new WindowsMediaPlayer();
            musicPlayer = new WindowsMediaPlayer();
            activeSound = sounds[3];
            paused = false;            
            

            A = new Peg(new Point(200, 480));
            B = new Peg(new Point(400, 480));
            C = new Peg(new Point(600, 480));
            Hanoi(rings, A, C, B);

            Point position = new Point(200, 480);
            int width = 150;
            for (int i = 0; i < rings; i++)
            {                
                A.Disks.Push(new Disk(colors[i], width, position, "A"));
                width -= 18;
                position.Y -= 20;
            }

            t = new Thread(new ThreadStart(Run));
            t.Start();            
        }


        private void Towers_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            Pen peg = new Pen(Color.Black, 5);

            graphics.DrawLine(peg, 200, 300, 200, 500);
            graphics.DrawLine(peg, 400, 300, 400, 500);
            graphics.DrawLine(peg, 600, 300, 600, 500);           

            DrawPeg(graphics, A);
            DrawPeg(graphics, B);
            DrawPeg(graphics, C);

                         
        }

        private void DrawPeg(Graphics graphics, Peg peg)
        {
            Disk[] disks = peg.Disks.ToArray();

            foreach(Disk disk in disks)
            {
                Brush brush = new SolidBrush(disk.Color);
                Point position = new Point(disk.Position.X - (disk.Width /2), disk.Position.Y);
                Rectangle rect = new Rectangle(position, new Size(disk.Width, 20));
                graphics.FillRectangle(brush, rect);               
            }

        }

        /* Hanoi
         * The recursive hanoi algorithm.
         */

        private void Hanoi(int numDisks, Peg source, Peg destination, Peg intermediate)
        {
            if (numDisks == 1)
            {
                moveList.Add(new NextMove(source, destination));
             
            }
            else
            {
                Hanoi(numDisks­ - 1, source, intermediate, destination);
                moveList.Add(new NextMove(source, destination));             
                Hanoi(numDisks­ - 1, intermediate, destination, source);
            }
        }


        /* Run
         * Threaded process for animating the sorting
         */
        private void Run()
        {
            foreach (NextMove move in moveList)
            {             
                active = move.Source.Disks.Peek();

                //move up
                while (active.Position.Y > 200)
                {
                    active.Position = new Point(active.Position.X, active.Position.Y - 1);
                    Invalidate();
                    Thread.Sleep(speed);
                    if (paused)
                    {
                        try
                        {
                            Thread.Sleep(Timeout.Infinite);
                        }
                        catch (ThreadInterruptedException) { };
                    }
                }

                //move right
                if (move.Destination.Position.X > move.Source.Position.X)
                {
                    while (active.Position.X < move.Destination.Position.X)
                    {
                        active.Position = new Point(active.Position.X + 1, active.Position.Y);
                        Invalidate();
                        Thread.Sleep(speed);
                        if (paused)
                        {
                            try
                            {
                                Thread.Sleep(Timeout.Infinite);
                            }
                            catch (ThreadInterruptedException) { };
                        }
                    }
                }
                //move left
                else
                {
                    while (active.Position.X > move.Destination.Position.X)
                    {
                        active.Position = new Point(active.Position.X - 1, active.Position.Y);
                        Invalidate();
                        Thread.Sleep(speed);
                        if (paused)
                        {
                            try
                            {
                                Thread.Sleep(Timeout.Infinite);
                            }
                            catch (ThreadInterruptedException) { };
                        }
                    }                  
                }

                //move down
                while(active.Position.Y < 480 - (move.Destination.Disks.Count * 20))
                {
                    active.Position = new Point(active.Position.X, active.Position.Y + 1);
                    Invalidate();
                    Thread.Sleep(speed);
                    if (paused)
                    {
                        try
                        {
                            Thread.Sleep(Timeout.Infinite);
                        }
                        catch (ThreadInterruptedException) { };
                    }
                }

                active.Position = new Point(move.Destination.Position.X, move.Destination.Position.Y - (move.Destination.Disks.Count * 20));                   

                move.Source.Disks.Pop();
                move.Destination.Disks.Push(active);

                if (btnSoundOff.Enabled) {
                    soundPlayer.URL = activeSound;
                }

            }

            InvokeUpdateControls();
        }

        public void InvokeUpdateControls()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateLbl(UpdateControls));
            }
            else
            {
                UpdateControls();
            }
        }

        public void UpdateControls()
        {
            lblComplete.Text = "Solved in " + moveList.Count + " move(s)!";
            lblComplete.Visible = true;
        }


        private void Towers_FormClosing(object sender, FormClosingEventArgs e)
        {
            soundPlayer.URL = null;
            musicPlayer.URL = null;
            t.Abort();
            Form parentForm = Application.OpenForms["Splash"] as Form;
            parentForm.Show();
        }


        private void scrSpeed_ValueChanged(object sender, EventArgs e)
        {
            speed = (scrSpeed.Value + 1);
        }

        private void scrSounds_ValueChanged(object sender, EventArgs e)
        {
            activeSound= sounds[scrSounds.Value];
        }

        private void btnSoundOn_Click(object sender, EventArgs e)
        {
            btnSoundOn.Enabled = false;
            btnSoundOff.Enabled = true;
            onToolStripMenuItem.Enabled = false;
            offToolStripMenuItem.Enabled = true;
            scrSounds.Enabled = false;    
        }

        private void btnSoundOff_Click(object sender, EventArgs e)
        {
            btnSoundOn.Enabled = true;
            btnSoundOff.Enabled = false;
            onToolStripMenuItem.Enabled = true;
            offToolStripMenuItem.Enabled = false;
            scrSounds.Enabled = true;
          
        }

        private void btnMusiOn_Click(object sender, EventArgs e)
        {
            btnMusiOn.Enabled = false;
            btnMusicOff.Enabled = true;
            onToolStripMenuItem1.Enabled = false;
            offToolStripMenuItem1.Enabled = true;
            musicPlayer.URL = music[scrMusi.Value];
            scrMusi.Enabled = false;
        }

        private void btnMusicOff_Click(object sender, EventArgs e)
        {
            btnMusiOn.Enabled = true;
            btnMusicOff.Enabled = false;
            onToolStripMenuItem1.Enabled = true;
            offToolStripMenuItem1.Enabled = false;
            musicPlayer.URL = null;
            scrMusi.Enabled = true;
        }

        private void chkPause_CheckedChanged(object sender, EventArgs e)
        {
            if (!paused)
            {
                paused = true;
                stopToolStripMenuItem.Enabled = false;
                resumeToolStripMenuItem.Enabled = true;
                chkPause.BackColor = SystemColors.ActiveCaption;                            
            }
            else
            {
                paused = false;
                stopToolStripMenuItem.Enabled = true;
                resumeToolStripMenuItem.Enabled = false;
                chkPause.BackColor = SystemColors.ControlLight;
                t.Interrupt();
            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void restToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t.Abort();
            lblComplete.Visible = false;
            moveList = new ArrayList();
            A = new Peg(new Point(200, 480));
            B = new Peg(new Point(400, 480));
            C = new Peg(new Point(600, 480));
            Hanoi(rings, A, C, B);
            Point position = new Point(200, 480);
            int width = 150;
            for (int i = 0; i < rings; i++)
            {
                A.Disks.Push(new Disk(colors[i], width, position, "A"));
                width -= 18;
                position.Y -= 20;
            }

            t = new Thread(new ThreadStart(Run));
            t.Start();
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkPause.Checked = true;
            MessageBox.Show("1.    You may only move the top disc of a stack of \n       discs(this also implies that you can only move one disc at a time)\n" +
                            "2.    A larger disc can never be placed on top of a smaller disc", "Rules", MessageBoxButtons.OK);
            chkPause.Checked = false;
        }

        private void mechanicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkPause.Checked = true;
            MessageBox.Show("Developer: Jesse Bosshardt\nVersion: 1.0.0\n.NET Framework: 4.5.2\nPlatform: 32 bit\n\nAll music composed and produced by Jesse Bosshardt", "About", MessageBoxButtons.OK);
            chkPause.Checked = false;
        }
    }    
}