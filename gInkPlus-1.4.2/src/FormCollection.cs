//using System.Windows.Input;
using Microsoft.Ink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace gInkPlus	
{
	public partial class FormCollection : Form
	{
		public Root Root;
		public InkOverlay IC;

		public Button[] btPen;
		public Bitmap image_exit, image_clear, image_undo, image_snap, image_penwidth;
		public Bitmap image_dock, image_dockback;
		public Bitmap image_pencil, image_highlighter, image_pencil_act, image_highlighter_act;

        // Added by mwtoolz; Line,Rect, Rect_filled tool
        public Button btLine;
        public Bitmap image_line, image_line_act;
		public Button btRect;
		public Bitmap image_rect, image_rect_act;
        public Button btRectFilled;
        public Bitmap image_rect_filled, image_rect_filled_act;
		// ===

        public Bitmap image_pointer, image_pointer_act;
		public Bitmap[] image_pen;
		public Bitmap[] image_pen_act;
		public Bitmap image_eraser_act, image_eraser;
		public Bitmap image_pan_act, image_pan;
		public Bitmap image_visible_not, image_visible;
		public System.Windows.Forms.Cursor cursorred, cursorsnap;
		public System.Windows.Forms.Cursor cursortip;

		public int ButtonsEntering = 0;  // -1 = exiting
		public int gpButtonsLeft, gpButtonsTop, gpButtonsWidth, gpButtonsHeight; // the default location, fixed

		public bool gpPenWidth_MouseOn = false;

		public int PrimaryLeft, PrimaryTop;

		// http://www.csharp411.com/hide-form-from-alttab/
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				// turn on WS_EX_TOOLWINDOW style bit
				cp.ExStyle |= 0x80;
				return cp;
			}
		}

		public FormCollection(Root root)
		{
			Root = root;
			InitializeComponent();

            // Added by mwtoolz
            this.FormClosed += FormCollection_FormClosed;    


            PrimaryLeft = Screen.PrimaryScreen.Bounds.Left - SystemInformation.VirtualScreen.Left;
			PrimaryTop = Screen.PrimaryScreen.Bounds.Top - SystemInformation.VirtualScreen.Top;     
            gpButtons.Height = (int)(Screen.PrimaryScreen.Bounds.Height * Root.ToolbarHeight);
			btClear.Height = (int)(gpButtons.Height * 0.88);
			btClear.Width = btClear.Height;
			btClear.Top = (int)(gpButtons.Height * 0.07);
			btDock.Height = (int)(gpButtons.Height * 0.88);
			btDock.Width = (int)(btDock.Height * 0.75);
			btDock.Top = (int)(gpButtons.Height * 0.07);
			btEraser.Height = (int)(gpButtons.Height * 0.88);
			btEraser.Width = btEraser.Height;
			btEraser.Top = (int)(gpButtons.Height * 0.07);
			btInkVisible.Height = (int)(gpButtons.Height * 0.88);
			btInkVisible.Width = btInkVisible.Height;
			btInkVisible.Top = (int)(gpButtons.Height * 0.07);
			btPan.Height = (int)(gpButtons.Height * 0.88);
			btPan.Width = btPan.Height;
			btPan.Top = (int)(gpButtons.Height * 0.07);
			btPointer.Height = (int)(gpButtons.Height * 0.88);
			btPointer.Width = btPointer.Height;
			btPointer.Top = (int)(gpButtons.Height * 0.07);
			btSnap.Height = (int)(gpButtons.Height * 0.88);
			btSnap.Width = btSnap.Height;
			btSnap.Top = (int)(gpButtons.Height * 0.07);
			btStop.Height = (int)(gpButtons.Height * 0.88);
			btStop.Width = btStop.Height;
			btStop.Top = (int)(gpButtons.Height * 0.07);
			btUndo.Height = (int)(gpButtons.Height * 0.88);
			btUndo.Width = btUndo.Height;
			btUndo.Top = (int)(gpButtons.Height * 0.07);

			btPen = new Button[Root.MaxPenCount];

			int cumulatedleft = (int)(btStop.Width * 1.2);
			for (int b = 0; b < Root.MaxPenCount; b++)
			{
				btPen[b] = new Button();
				btPen[b].Width = (int)(gpButtons.Height * 0.88);
				btPen[b].Height = (int)(gpButtons.Height * 0.88);
				btPen[b].Top = (int)(gpButtons.Height * 0.08);
				btPen[b].FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
				btPen[b].FlatAppearance.BorderSize = 3;
				btPen[b].FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(250, 50, 50);
				btPen[b].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				btPen[b].ForeColor = System.Drawing.Color.Transparent;
				//btPen[b].Name = "btPen" + b.ToString();
				btPen[b].UseVisualStyleBackColor = false;
				btPen[b].Click += new System.EventHandler(this.btColor_Click);
				btPen[b].BackColor = Root.PenAttr[b].Color;
				btPen[b].FlatAppearance.MouseDownBackColor = Root.PenAttr[b].Color;
				btPen[b].FlatAppearance.MouseOverBackColor = Root.PenAttr[b].Color;

				this.toolTip.SetToolTip(this.btPen[b], Root.Local.ButtonNamePen[b] + " (" + Root.Hotkey_Pens[b].ToString() + ")");

				btPen[b].MouseDown += gpButtons_MouseDown;
				btPen[b].MouseMove += gpButtons_MouseMove;
				btPen[b].MouseUp += gpButtons_MouseUp;

				gpButtons.Controls.Add(btPen[b]);

				if (Root.PenEnabled[b])
				{
					btPen[b].Visible = true;
					btPen[b].Left = cumulatedleft;
					cumulatedleft += (int)(btPen[b].Width * 1.1);
				}
				else
				{
					btPen[b].Visible = false;
				}
			}
			cumulatedleft += (int)(btStop.Width * 0.8);
			if (Root.EraserEnabled)
			{
				btEraser.Visible = true;
				btEraser.Left = cumulatedleft;
				cumulatedleft += (int)(btEraser.Width * 1.1);
			}
			else
			{
				btEraser.Visible = false;
			}

            //Added by mwtoolz for Shape tools Button Attributes
            btLine = new Button();
            SetupShapeButton(btLine, this.btLine_Click, ref cumulatedleft);

            btRect = new Button();
            SetupShapeButton(btRect, this.btRect_Click, ref cumulatedleft);

            btRectFilled = new Button();
            SetupShapeButton(btRectFilled, this.btRectFilled_Click, ref cumulatedleft);

			// Replacing code with Helper function SetupShapeButtons
			/*
            // ===== LINE BUTTON =====
			
            btLine = new Button();
            btLine.Width = (int)(gpButtons.Height * 0.88);
            btLine.Height = (int)(gpButtons.Height * 0.88);
            btLine.Top = (int)(gpButtons.Height * 0.07);
            btLine.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            btLine.FlatAppearance.BorderSize = 3;
            btLine.FlatStyle = FlatStyle.Flat;

            // Prevent grey background on hover and click
            btLine.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btLine.FlatAppearance.MouseDownBackColor = Color.Transparent;

            btLine.ForeColor = Color.Transparent;
            btLine.UseVisualStyleBackColor = false;
            btLine.Click += new EventHandler(this.btLine_Click);
            btLine.ImageAlign = ContentAlignment.MiddleCenter;
            gpButtons.Controls.Add(btLine);
            btLine.Visible = true;
            btLine.Left = cumulatedleft;
            cumulatedleft += (int)(btLine.Width * 1.1);
            // ===== END LINE BUTTON =====

            //Added by mwtoolz
            // ===== RECT BUTTON =====
            btRect = new Button();
            btRect.Width = (int)(gpButtons.Height * 0.88);
            btRect.Height = (int)(gpButtons.Height * 0.88);
            btRect.Top = (int)(gpButtons.Height * 0.07);
            btRect.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            btRect.FlatAppearance.BorderSize = 3;
            btRect.FlatStyle = FlatStyle.Flat;

            // Prevent grey background on hover and click
            btRect.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btRect.FlatAppearance.MouseDownBackColor = Color.Transparent;

            btRect.ForeColor = Color.Transparent;
            btRect.UseVisualStyleBackColor = false;
            btRect.Click += new EventHandler(this.btRect_Click);
            btRect.ImageAlign = ContentAlignment.MiddleCenter;
            gpButtons.Controls.Add(btRect);
            btRect.Visible = true;
            btRect.Left = cumulatedleft;
            cumulatedleft += (int)(btRect.Width * 1.1);
            // ===== END RECTANGLE BUTTON =====

            //Added by mwtoolz
            // ===== RECT_FILLED BUTTON =====            
            btRectFilled = new Button();
            btRectFilled.Width = (int)(gpButtons.Height * 0.88);
            btRectFilled.Height = (int)(gpButtons.Height * 0.88);
            btRectFilled.Top = (int)(gpButtons.Height * 0.07);
            btRectFilled.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            btRectFilled.FlatAppearance.BorderSize = 3;
            btRectFilled.FlatStyle = FlatStyle.Flat;// Prevent grey background on hover and click
            btRectFilled.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btRectFilled.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btRectFilled.ForeColor = Color.Transparent;
            btRectFilled.UseVisualStyleBackColor = false;
            btRectFilled.Click += new EventHandler(this.btRectFilled_Click);
            btRectFilled.ImageAlign = ContentAlignment.MiddleCenter;
            gpButtons.Controls.Add(btRectFilled);
            btRectFilled.Visible = true;
            btRectFilled.Left = cumulatedleft;
			cumulatedleft += (int)(btRectFilled.Width * 1.1);
            // ===== END RECT_FILLED BUTTON =====
			*/
			
            if (Root.PanEnabled)
			{
				btPan.Visible = true;
				btPan.Left = cumulatedleft;
				cumulatedleft += (int)(btPan.Width * 1.1);
			}
			else
			{
				btPan.Visible = false;
			}
			if (Root.PointerEnabled)
			{
				btPointer.Visible = true;
				btPointer.Left = cumulatedleft;
				cumulatedleft += (int)(btPointer.Width * 1.1);
			}
			else
			{
				btPointer.Visible = false;
			}
			cumulatedleft += (int)(btStop.Width * 0.8);

			if (Root.PenWidthEnabled)
			{
				btPenWidth.Visible = true;
				btPenWidth.Left = cumulatedleft;
				cumulatedleft += (int)(btPenWidth.Width * 1.1);
			}
			else
			{
				btPenWidth.Visible = false;
			}
			if (Root.InkVisibleEnabled)
			{
				btInkVisible.Visible = true;
				btInkVisible.Left = cumulatedleft;
				cumulatedleft += (int)(btInkVisible.Width * 1.1);
			}
			else
			{
				btInkVisible.Visible = false;
			}
			if (Root.SnapEnabled)
			{
				btSnap.Visible = true;
				btSnap.Left = cumulatedleft;
				cumulatedleft += (int)(btSnap.Width * 1.1);
			}
			else
			{
				btSnap.Visible = false;
			}
			if (Root.UndoEnabled)
			{
				btUndo.Visible = true;
				btUndo.Left = cumulatedleft;
				cumulatedleft += (int)(btUndo.Width * 1.1);
			}
			else
			{
				btUndo.Visible = false;
			}
			if (Root.ClearEnabled)
			{
				btClear.Visible = true;
				btClear.Left = cumulatedleft;
				cumulatedleft += (int)(btClear.Width * 1.1);
			}
			else
			{
				btClear.Visible = false;
			}
			cumulatedleft += (int)(btStop.Width * 0.8);
			btStop.Left = cumulatedleft;
			gpButtons.Width = btStop.Right + (int)(btStop.Width * 0.5);
			

			this.Left = SystemInformation.VirtualScreen.Left;
			this.Top = SystemInformation.VirtualScreen.Top;
			//int targetbottom = 0;
			//foreach (Screen screen in Screen.AllScreens)
			//{
			//	if (screen.WorkingArea.Bottom > targetbottom)
			//		targetbottom = screen.WorkingArea.Bottom;
			//}
			//int virwidth = SystemInformation.VirtualScreen.Width;
			//this.Width = virwidth;
			//this.Height = targetbottom - this.Top;
			this.Width = SystemInformation.VirtualScreen.Width;
			this.Height = SystemInformation.VirtualScreen.Height - 2;
			this.DoubleBuffered = true;

			gpButtonsWidth = gpButtons.Width;
			gpButtonsHeight = gpButtons.Height;
			if (true || Root.AllowDraggingToolbar)
			{
				gpButtonsLeft = Root.gpButtonsLeft;
				gpButtonsTop = Root.gpButtonsTop;
				if
				(
					!(IsInsideVisibleScreen(gpButtonsLeft, gpButtonsTop) &&
					IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth, gpButtonsTop) &&
					IsInsideVisibleScreen(gpButtonsLeft, gpButtonsTop + gpButtonsHeight) &&
					IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth, gpButtonsTop + gpButtonsHeight))
					||
					(gpButtonsLeft == 0 && gpButtonsTop == 0)
				)
				{
					gpButtonsLeft = Screen.PrimaryScreen.WorkingArea.Right - gpButtons.Width + PrimaryLeft;
					gpButtonsTop = Screen.PrimaryScreen.WorkingArea.Bottom - gpButtons.Height - 15 + PrimaryTop;
				}
			}
			else
			{
				gpButtonsLeft = Screen.PrimaryScreen.WorkingArea.Right - gpButtons.Width + PrimaryLeft;
				gpButtonsTop = Screen.PrimaryScreen.WorkingArea.Bottom - gpButtons.Height - 15 + PrimaryTop;
			}

			gpButtons.Left = gpButtonsLeft + gpButtons.Width;
			gpButtons.Top = gpButtonsTop;
			gpPenWidth.Left = gpButtonsLeft + btPenWidth.Left - gpPenWidth.Width / 2 + btPenWidth.Width / 2;
			gpPenWidth.Top = gpButtonsTop - gpPenWidth.Height - 10;

			pboxPenWidthIndicator.Top = 0;
			pboxPenWidthIndicator.Left = (int)Math.Sqrt(Root.GlobalPenWidth * 30);			
			
			gpPenWidth.Controls.Add(pboxPenWidthIndicator);

			IC = new InkOverlay(this.Handle);
			IC.CollectionMode = CollectionMode.InkOnly;
			IC.AutoRedraw = false;
			IC.DynamicRendering = false;
			IC.EraserMode = InkOverlayEraserMode.StrokeErase;
			
			IC.CursorInRange += IC_CursorInRange;
			IC.MouseDown += IC_MouseDown;
			IC.MouseMove += IC_MouseMove;
			IC.MouseUp += IC_MouseUp;
			IC.CursorDown += IC_CursorDown;
			IC.Stroke += IC_Stroke;
		
            IC.DefaultDrawingAttributes.Width = 80;
			IC.DefaultDrawingAttributes.Transparency = 30;
			IC.DefaultDrawingAttributes.AntiAliased = true;

			cursorred = new System.Windows.Forms.Cursor(gInkPlus.Properties.Resources.cursorred.Handle);
			//IC.Cursor = cursorred;
			IC.Enabled = true;

			// Modified by mwtoolz- Readability; replaced with helper method CreateToolbarImage() that implements graphics g
			// ===
			image_exit = CreateToolbarImage(global::gInkPlus.Properties.Resources.exit, btStop);
			btStop.Image = image_exit;

            image_clear = CreateToolbarImage(global::gInkPlus.Properties.Resources.garbage, btClear);
			btClear.Image = image_clear;

            image_undo = CreateToolbarImage(global::gInkPlus.Properties.Resources.undo, btUndo);
			btUndo.Image = image_undo;

            image_eraser_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.eraser_act, btEraser);
            image_eraser = CreateToolbarImage(global::gInkPlus.Properties.Resources.eraser, btEraser);
			btEraser.Image = image_eraser;

            image_pan_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.pan_act, btPan);
            image_pan = CreateToolbarImage(global::gInkPlus.Properties.Resources.pan, btPan);
            btPan.Image = image_pan;

            image_visible_not = CreateToolbarImage(global::gInkPlus.Properties.Resources.visible_not, btInkVisible);
            image_visible = CreateToolbarImage(global::gInkPlus.Properties.Resources.visible, btInkVisible);
            btInkVisible.Image = image_visible;

            image_snap = CreateToolbarImage(global::gInkPlus.Properties.Resources.snap, btSnap);
            btSnap.Image = image_snap;

            image_penwidth = CreateToolbarImage(global::gInkPlus.Properties.Resources.penwidth, btPenWidth);
            btPenWidth.Image = image_penwidth;

            image_dock = CreateToolbarImage(global::gInkPlus.Properties.Resources.dock, btDock);
            image_dockback = CreateToolbarImage(global::gInkPlus.Properties.Resources.dockback, btDock);
			if (Root.Docked) btDock.Image = image_dockback; else btDock.Image = image_dock;
            
			image_pencil = CreateToolbarImage(global::gInkPlus.Properties.Resources.pencil, btPen[2]);
            image_pencil_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.pencil_act, btPen[2]);
            image_highlighter = CreateToolbarImage(global::gInkPlus.Properties.Resources.highlighter, btPen[2]);
            image_highlighter_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.highlighter_act, btPen[2]);

			image_pointer = CreateToolbarImage(global::gInkPlus.Properties.Resources.pointer, btPointer);
            image_pointer_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.pointer_act, btPointer);

            // Added by mwtoolz- Shape Tools
            image_line_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.tool_line_act, btLine);
            image_line = CreateToolbarImage(global::gInkPlus.Properties.Resources.tool_line, btLine);
			btLine.Image = image_line;

			image_rect_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.tool_rect_act, btRect);
            image_rect = CreateToolbarImage(global::gInkPlus.Properties.Resources.tool_rect, btRect);
			btRect.Image = image_rect;

            image_rect_filled_act = CreateToolbarImage(global::gInkPlus.Properties.Resources.tool_rect_filled_act, btRectFilled);
            image_rect_filled = CreateToolbarImage(global::gInkPlus.Properties.Resources.tool_rect_filled, btRectFilled);
            btRectFilled.Image = image_rect_filled;



			// ===

			image_pen = new Bitmap[Root.MaxPenCount];
			image_pen_act = new Bitmap[Root.MaxPenCount];

			for (int b = 0; b < Root.MaxPenCount; b++)
			{
				if (Root.PenAttr[b].Transparency >= 100)
				{

					// Modified by mwtoolz; Memory Leak here
					// image_pen[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen[b] = image_highlighter;
					// image_pen_act[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen_act[b] = image_highlighter_act;
				}
				else
				{
					// image_pen[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen[b] = image_pencil;
					// image_pen_act[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen_act[b] = image_pencil_act;
				}
			}

			LastTickTime = DateTime.Parse("1987-01-01");
			tiSlide.Enabled = true;

			ToTransparent();
			ToTopMost();

			this.toolTip.SetToolTip(this.btDock, Root.Local.ButtonNameDock);
			this.toolTip.SetToolTip(this.btPenWidth, Root.Local.ButtonNamePenwidth);
			this.toolTip.SetToolTip(this.btEraser, Root.Local.ButtonNameErasor + " (" + Root.Hotkey_Eraser.ToString() + ")");
			this.toolTip.SetToolTip(this.btPan, Root.Local.ButtonNamePan + " (" + Root.Hotkey_Pan.ToString() + ")");
			this.toolTip.SetToolTip(this.btPointer, Root.Local.ButtonNameMousePointer + " (" + Root.Hotkey_Pointer.ToString() + ")");
			this.toolTip.SetToolTip(this.btInkVisible, Root.Local.ButtonNameInkVisible + " (" + Root.Hotkey_InkVisible.ToString() + ")");
			this.toolTip.SetToolTip(this.btSnap, Root.Local.ButtonNameSnapshot + " (" + Root.Hotkey_Snap.ToString() + ")");
			this.toolTip.SetToolTip(this.btUndo, Root.Local.ButtonNameUndo + " (" + Root.Hotkey_Undo.ToString() + ")");
			this.toolTip.SetToolTip(this.btClear, Root.Local.ButtonNameClear + " (" + Root.Hotkey_Clear.ToString() + ")");
			this.toolTip.SetToolTip(this.btStop, Root.Local.ButtonNameExit + " (ESC)");

			// TODO by mwtoolz for Shape Tools


		}

        // Added by mwtoolz- Readability; Helper method for Shape Tool Button Attributes
        private void SetupShapeButton(Button button, EventHandler clickHandler, ref int cumulatedleft)
        {
            button.Width = (int)(gpButtons.Height * 0.88);
            button.Height = (int)(gpButtons.Height * 0.88);
            button.Top = (int)(gpButtons.Height * 0.07);

            button.FlatAppearance.BorderColor = Color.WhiteSmoke;
            button.FlatAppearance.BorderSize = 3;
            button.FlatStyle = FlatStyle.Flat;

            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;

            button.ForeColor = Color.Transparent;
            button.UseVisualStyleBackColor = false;
            button.ImageAlign = ContentAlignment.MiddleCenter;

            button.Click += clickHandler;
            gpButtons.Controls.Add(button);

            // TODO by mwtoolz for Tab 1- General form options
            button.Visible = true;
            button.Left = cumulatedleft;
            cumulatedleft += (int)(button.Width * 1.1);
        }


        // Added by mwtoolz- Readability; Helper method CreateToolbarImage()
        private Bitmap CreateToolbarImage(Image resource, Button button)
        {
            Bitmap image = new Bitmap(button.Width, button.Height);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(resource, 0, 0, button.Width, button.Height);
            }

            return image;
        }

        private void IC_Stroke(object sender, InkCollectorStrokeEventArgs e)
		{

            // Added by mwtoolz        
            // Line & Rect mode: discard the unwanted native freehand stroke.
            if (Root.IsShapeMode && Root.FormDisplay.IsDrawingGeometricShape)
			{ 
                IC.Ink.DeleteStroke(e.Stroke);
                return;
            }
			SaveUndoStrokes();
		}

		private void SaveUndoStrokes()
		{
			Root.RedoDepth = 0;
			if (Root.UndoDepth < Root.UndoStrokes.GetLength(0) - 1)
				Root.UndoDepth++;

			Root.UndoP++;
			if (Root.UndoP >= Root.UndoStrokes.GetLength(0))
				Root.UndoP = 0;

			if (Root.UndoStrokes[Root.UndoP] == null)
				Root.UndoStrokes[Root.UndoP] = new Ink();
			Root.UndoStrokes[Root.UndoP].DeleteStrokes();
			if (IC.Ink.Strokes.Count > 0)
				Root.UndoStrokes[Root.UndoP].AddStrokesAtRectangle(IC.Ink.Strokes, IC.Ink.Strokes.GetBoundingBox());
		}

		private void IC_CursorDown(object sender, InkCollectorCursorDownEventArgs e)
		{
            // Added by mwtoolz     
            if (!Root.InkVisible && Root.Snapping <= 0)
			{
				Root.SetInkVisible(true);
			}

			// Added by mwtoolz
			// Skip native ink buffer preparation during geometric shape drawing
			if (Root.IsShapeMode && Root.FormDisplay.IsDrawingGeometricShape) return;

			Root.FormDisplay.ClearCanvus(Root.FormDisplay.gOneStrokeCanvus);
			Root.FormDisplay.DrawStrokes(Root.FormDisplay.gOneStrokeCanvus);
			Root.FormDisplay.DrawButtons(Root.FormDisplay.gOneStrokeCanvus, false);
		}

        private void IC_MouseDown(object sender, CancelMouseEventArgs e)
		{

            // Added by mwtoolz, Gemini	
            if (Root.LineMode)
            {
                Root.FormDisplay.IsDrawingLine = true;
                Root.FormDisplay.LineStartPoint = new Point(e.X, e.Y);
                Root.FormDisplay.CurrentMousePoint = Root.FormDisplay.LineStartPoint;
                Root.FormDisplay.oldBoxValid = false;

                // CRITICAL: Prepare the background buffer (onestrokeDc / gOneStrokeCanvus) 
                // so BitBlt has a clean snapshot to restore from during the ticker loop.
                Root.FormDisplay.ClearCanvus(Root.FormDisplay.gOneStrokeCanvus);
                Root.FormDisplay.DrawStrokes(Root.FormDisplay.gOneStrokeCanvus);
                Root.FormDisplay.DrawButtons(Root.FormDisplay.gOneStrokeCanvus, false);

                // CRITICAL: Prevent gInkPlus from starting a normal freehand pencil stroke
                e.Cancel = true;
                return;
			}

			// Rectangle Mode
			if (Root.RectMode)
			{
				Root.FormDisplay.IsDrawingRectangle = true;
				Root.FormDisplay.RectStartPoint = new Point(e.X, e.Y);
				Root.FormDisplay.CurrentMousePoint = Root.FormDisplay.RectStartPoint;
				Root.FormDisplay.oldRectBoxValid = false;

				Root.FormDisplay.ClearCanvus(Root.FormDisplay.gOneStrokeCanvus);
				Root.FormDisplay.DrawStrokes(Root.FormDisplay.gOneStrokeCanvus);
				Root.FormDisplay.DrawButtons(Root.FormDisplay.gOneStrokeCanvus, false);

				e.Cancel = true;
				return;
			}

            // RectFilled Mode
            if (Root.RectFilledMode)
            {
                Root.FormDisplay.IsDrawingRectFilled = true; // Or a unified flag like IsDrawingRectangle = true with a shape type
                Root.FormDisplay.RectStartPoint = new Point(e.X, e.Y);
                Root.FormDisplay.CurrentMousePoint = Root.FormDisplay.RectStartPoint;
                Root.FormDisplay.oldRectBoxValid = false;

                Root.FormDisplay.ClearCanvus(Root.FormDisplay.gOneStrokeCanvus);
                Root.FormDisplay.DrawStrokes(Root.FormDisplay.gOneStrokeCanvus);
                Root.FormDisplay.DrawButtons(Root.FormDisplay.gOneStrokeCanvus, false);

                e.Cancel = true;
                return;
            }

            // ======

            if (Root.gpPenWidthVisible)
			{
				Root.gpPenWidthVisible = false;
				Root.UponSubPanelUpdate = true;
			}

			Root.FingerInAction = true;
			if (Root.Snapping == 1)
			{
				Root.SnappingX = e.X;
				Root.SnappingY = e.Y;
				Root.SnappingRect = new Rectangle(e.X, e.Y, 0, 0);
				Root.Snapping = 2;
			}

			if (!Root.InkVisible && Root.Snapping <= 0)
			{
				Root.SetInkVisible(true);
			}

			LasteXY.X = e.X;
			LasteXY.Y = e.Y;
			IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref LasteXY);
		}

		public Point LasteXY;


        private void IC_MouseMove(object sender, CancelMouseEventArgs e)
		{

			// Added by mwtoolz
			// New Code
			// Line, Rect, RectFilled Mode Tracking
			if (Root.IsShapeMode && Root.FormDisplay.IsDrawingGeometricShape)
			{
                Root.FormDisplay.CurrentMousePoint = new Point(e.X, e.Y);
                e.Cancel = true;
                return;
            }

            // ====

            if (LasteXY.X == 0 && LasteXY.Y == 0)
			{
				LasteXY.X = e.X;
				LasteXY.Y = e.Y;
				IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref LasteXY);
			}
			Point currentxy = new Point(e.X, e.Y);
			IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref currentxy);

			if (Root.Snapping == 2)
			{
				int left = Math.Min(Root.SnappingX, e.X);
				int top = Math.Min(Root.SnappingY, e.Y);
				int width = Math.Abs(Root.SnappingX - e.X);
				int height = Math.Abs(Root.SnappingY - e.Y);
				Root.SnappingRect = new Rectangle(left, top, width, height);

				if (LasteXY != currentxy)
					Root.MouseMovedUnderSnapshotDragging = true;
			}
			else if (Root.PanMode && Root.FingerInAction)
			{
				Root.Pan(currentxy.X - LasteXY.X, currentxy.Y - LasteXY.Y);
            }

			LasteXY = currentxy;
		}

		private void IC_MouseUp(object sender, CancelMouseEventArgs e)
		{

            // Added by mwtoolz, Gemini            
			// ===== LINE TOOL - CREATE FINAL STROKE =====
            if (Root.LineMode && Root.FormDisplay.IsDrawingLine)
            {
                Root.FormDisplay.ResetLineDrawingState();

                // 1. Convert pixel points to ink space coordinates
                Point startInk = new Point(Root.FormDisplay.LineStartPoint.X, Root.FormDisplay.LineStartPoint.Y);
                IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref startInk);

                Point endInk = new Point(Root.FormDisplay.CurrentMousePoint.X, Root.FormDisplay.CurrentMousePoint.Y);
                IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref endInk);

                // 2. Create the permanent Microsoft.Ink stroke
                Point[] points = new Point[]
                {new Point(startInk.X, startInk.Y), new Point(endInk.X, endInk.Y)};
                Stroke lineStroke = IC.Ink.CreateStroke(points);

                // 3. Apply current pen attributes
                if (Root.CurrentPen >= 0 && Root.CurrentPen < Root.PenAttr.Length)
                {
                    lineStroke.DrawingAttributes = Root.PenAttr[Root.CurrentPen].Clone();
                }
                else
                {
                    lineStroke.DrawingAttributes = IC.DefaultDrawingAttributes.Clone();
                }	

                // 4. Save to undo history and refresh the display cleanly
                SaveUndoStrokes();

                Root.FormDisplay.ClearCanvus();
                Root.FormDisplay.DrawStrokes();
                Root.FormDisplay.DrawButtons(true);
                Root.FormDisplay.UpdateFormDisplay(true);
          
                e.Cancel = true;
                return;
            }
            // ===== END LINE TOOL =====

            // ===== RECTANGLE & RECT FILLED TOOL - CREATE FINAL STROKE =====
            if ((Root.RectMode && Root.FormDisplay.IsDrawingRectangle) ||
				(Root.RectFilledMode && Root.FormDisplay.IsDrawingRectFilled))
            {
                // Determine if this specific shape is filled based on which mode is active
                bool isFilled = Root.RectFilledMode;

				if (!isFilled) 
				{ 
					Root.FormDisplay.ResetRectDrawingState();
				}
				else
				{
                    Root.FormDisplay.ResetRectFilledDrawingState();
                }				

                // 1. Calculate rectangle bounds in pixels (handles any drag direction)
                int rectX = Math.Min(Root.FormDisplay.RectStartPoint.X, Root.FormDisplay.CurrentMousePoint.X);
                int rectY = Math.Min(Root.FormDisplay.RectStartPoint.Y, Root.FormDisplay.CurrentMousePoint.Y);
                int rectWidth = Math.Abs(Root.FormDisplay.CurrentMousePoint.X - Root.FormDisplay.RectStartPoint.X);
                int rectHeight = Math.Abs(Root.FormDisplay.CurrentMousePoint.Y - Root.FormDisplay.RectStartPoint.Y);

                // Define the 4 corners + 1 to close the rectangle loop
                Point p1 = new Point(rectX, rectY);                          // Top-Left
                Point p2 = new Point(rectX + rectWidth, rectY);              // Top-Right
                Point p3 = new Point(rectX + rectWidth, rectY + rectHeight); // Bottom-Right
                Point p4 = new Point(rectX, rectY + rectHeight);             // Bottom-Left
                Point p5 = new Point(rectX, rectY);                          // Back to Top-Left to close

                // 2. Convert pixel points to ink space coordinates
                IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref p1);
                IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref p2);
                IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref p3);
                IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref p4);
                IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref p5);

                // 3. Create the permanent Microsoft.Ink stroke using the 5 points
                Point[] points = new Point[] { p1, p2, p3, p4, p5 };
                Stroke rectStroke = IC.Ink.CreateStroke(points);           

                // 5. Apply fill property ONLY if it is the RectFilled mode
                if (isFilled)
                {                   
                    rectStroke.ExtendedProperties.Add(Root.FilledShapeGuid, true);
                }

                // 4. Apply current pen attributes
                if (Root.CurrentPen >= 0 && Root.CurrentPen < Root.PenAttr.Length)
                {
                    rectStroke.DrawingAttributes = Root.PenAttr[Root.CurrentPen].Clone();
                }
                else
                {
                    rectStroke.DrawingAttributes = IC.DefaultDrawingAttributes.Clone();
                }


                // 6. Save to undo history and refresh the display cleanly
                SaveUndoStrokes();

                Root.FormDisplay.ClearCanvus();
                Root.FormDisplay.DrawStrokes();
                Root.FormDisplay.DrawButtons(true);
                Root.FormDisplay.UpdateFormDisplay(true);

                e.Cancel = true;
                return;
            }
            // ===== END RECTANGLE & RECT FILLED TOOL =====


            Root.FingerInAction = false;
			if (Root.Snapping == 2)
			{
				int left = Math.Min(Root.SnappingX, e.X);
				int top = Math.Min(Root.SnappingY, e.Y);
				int width = Math.Abs(Root.SnappingX - e.X);
				int height = Math.Abs(Root.SnappingY - e.Y);
				if (width < 5 || height < 5)
				{
					left = 0;
					top = 0;
					width = this.Width;
					height = this.Height;
				}
				Root.SnappingRect = new Rectangle(left + this.Left, top + this.Top, width, height);
				Root.UponTakingSnap = true;
				ExitSnapping();
			}
			else if (Root.PanMode)
			{
				SaveUndoStrokes();
			}
			else
			{
				Root.UponAllDrawingUpdate = true;
			}
		}

		private void IC_CursorInRange(object sender, InkCollectorCursorInRangeEventArgs e)
		{
			if (e.Cursor.Inverted && Root.CurrentPen != -1)
			{
				EnterEraserMode(true);
				/*
				// temperary eraser icon light
				if (btEraser.Image == image_eraser)
				{
					btEraser.Image = image_eraser_act;
					Root.FormDisplay.DrawButtons(true);
					Root.FormDisplay.UpdateFormDisplay();
				}
				*/
			}
			else if (!e.Cursor.Inverted && Root.CurrentPen != -1)
			{
				EnterEraserMode(false);
				/*
				if (btEraser.Image == image_eraser_act)
				{
					btEraser.Image = image_eraser;
					Root.FormDisplay.DrawButtons(true);
					Root.FormDisplay.UpdateFormDisplay();
				}
				*/
			}
		}

		public void ToTransparent()
		{
			UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
			SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
			SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 1, 0x2);
		}

		public void ToTopMost()
		{
			SetWindowPos(this.Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0020);
		}

		public void ToThrough()
		{
			UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
			//SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
			//SetWindowPos(this.Handle, (IntPtr)0, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0004 | 0x0010 | 0x0020);
			//SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 1, 0x2);
			SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000 | 0x00000020);
			//SetWindowPos(this.Handle, (IntPtr)(1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0020);
		}

		public void ToUnThrough()
		{
			UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
			//SetWindowLong(this.Handle, -20, (uint)(dwExStyle & ~0x00080000 & ~0x0020));
			SetWindowLong(this.Handle, -20, (uint)(dwExStyle & ~0x0020));
			//SetWindowPos(this.Handle, (IntPtr)(-2), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0020);

			//dwExStyle = GetWindowLong(this.Handle, -20);
			//SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
			//SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 1, 0x2);
			//SetWindowPos(this.Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0020);
		}

		public void EnterEraserMode(bool enter)
		{
			int exceptiontick = 0;
			bool exc;
			do
			{
				exceptiontick++;
				exc = false;
				try
				{
					if (enter)
					{
						IC.EditingMode = InkOverlayEditingMode.Delete;
						Root.EraserMode = true;
					}
					else
					{
						IC.EditingMode = InkOverlayEditingMode.Ink;
						Root.EraserMode = false;
					}
				}
				catch
				{
					Thread.Sleep(50);
					exc = true;
				}
			}
			while (exc && exceptiontick < 3);
		}

		public void SelectPen(int pen)
		{

            // Added by mwtoolz
            // -4=line, -5=rectangle, -6=rect_filled

            // Reset all existing tool states and toolbar images
            ResetShapeToolsMode();
            ResetGeneralToolsMode();

            // ===== RECT_FILLED TOOL =====
            if (pen == -6)
			{
                // Activate RectFilled Mode
                btRectFilled.Image = image_rect_filled_act;
                Root.RectFilledMode = true;

				// Set Cursor
				SetCanvasCursor();

                // Keep FULL input area so we still receive mouse events
                SetFullInputArea();

            }
            // ===== END OF RECT_FILLED TOOL =====        

            // ===== RECTANGLE TOOL =====
            else if (pen == -5)
            { 
                // Activate Rectangle mode
                btRect.Image = image_rect_act;
                Root.RectMode = true;

				// Set Cursor
				SetCanvasCursor();

                // Keep FULL input area so we still receive mouse events
                SetFullInputArea();

            }
            // ===== END OF RECTANGLE TOOL =====

            // ===== LINE TOOL =====
            else if (pen == -4)   
            {   
				// Activate Line tool
                btLine.Image = image_line_act;
                Root.LineMode = true;

                // Set Cursor
                SetCanvasCursor();

                // Keep FULL input area so we still receive mouse events
                SetFullInputArea();
            }

            // ==== END OF LINE TOOL ====
            
            // -3=pan, -2=pointer, -1=eraser, 0+=pens	
            else if (pen == -3)
			{         
                // Activate Pan tool
                btPan.Image = image_pan_act;
                Root.PanMode = true;

                SetInkInputRectangle(new Rectangle(0, 0, 1, 1));

            }
			else if (pen == -2)
			{
				// Activate Pointer tool
				btPointer.Image = image_pointer_act;
                Root.Pointer();
            }
			else if (pen == -1)
			{
				if (this.Cursor != System.Windows.Forms.Cursors.Default)
					this.Cursor = System.Windows.Forms.Cursors.Default;

				// Activate Eraser tool
				btEraser.Image = image_eraser_act;
                EnterEraserMode(true);

                // Modified by mwtoolz- set cursor
                // Set Cursor
                SetCanvasCursor();
                SetFullInputArea();

            }
			else if (pen >= 0)
			{
				if (this.Cursor != System.Windows.Forms.Cursors.Default) this.Cursor = System.Windows.Forms.Cursors.Default;

				IC.DefaultDrawingAttributes = Root.PenAttr[pen].Clone();
				if (Root.PenWidthEnabled) IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;				

				// Activate Selected Pen tool
				btPen[pen].Image = image_pen_act[pen];

                // Modified by mwtoolz- cursor
                // Set Cursor
                SetCanvasCursor();
                SetFullInputArea();

            }

			Root.CurrentPen = pen;

			if (Root.gpPenWidthVisible)
			{
				Root.gpPenWidthVisible = false;
				Root.UponSubPanelUpdate = true;
			}
			else Root.UponButtonsUpdate |= 0x2;

			if (pen != -2) Root.LastPen = pen;
		}

		public void RetreatAndExit()
		{
			ToThrough();
			Root.ClearInk();
			SaveUndoStrokes();
			Root.SaveOptions("config.ini");
			Root.gpPenWidthVisible = false;

			LastTickTime = DateTime.Now;
			ButtonsEntering = -9;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		public void btDock_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			LastTickTime = DateTime.Now;
			if (!Root.Docked) Root.Dock();
			else Root.UnDock();
		}

		public void btPointer_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			SelectPen(-2);
		}


		private void btPenWidth_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			if (Root.PointerMode)
				return;

			Root.gpPenWidthVisible = !Root.gpPenWidthVisible;
			if (Root.gpPenWidthVisible)
				Root.UponButtonsUpdate |= 0x2;
			else
				Root.UponSubPanelUpdate = true;
		}

        // Added by mwtoolz
		// == Line tool ==
        private void btLine_Click(object sender, EventArgs e)
        {
            SelectPen(-4);   // -4 is the value we defined for Line mode
        }        
        // == Rectangle tool ==
        private void btRect_Click(object sender, EventArgs e)
        {
            SelectPen(-5);   // -5 is the value we defined for Rectangle mode
        }
        // == Rect_Filled tool
        private void btRectFilled_Click(object sender, EventArgs e)
        {
            SelectPen(-6);   // -6 is the value we defined for Rect_Filled mode
        }

        //==



        public void btSnap_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			if (Root.Snapping > 0)
				return;

			cursorsnap = new System.Windows.Forms.Cursor(gInkPlus.Properties.Resources.cursorsnap.Handle);
			this.Cursor = cursorsnap;

			Root.gpPenWidthVisible = false;

            // Added by mwtoolz- Cancel any active shape drawing mode
            ResetShapeToolsMode();

            // Added by mwtoolz           
            SetInkInputRectangle(new Rectangle(0, 0, 1, 1));   

            Root.SnappingX = -1;
			Root.SnappingY = -1;
			Root.SnappingRect = new Rectangle(0, 0, 0, 0);
			Root.Snapping = 1;
			ButtonsEntering = -2;
			Root.UnPointer();
		}

		public void ExitSnapping()
		{

			SetFullInputArea();

            Root.SnappingX = -1;
			Root.SnappingY = -1;
			Root.Snapping = -60;
			ButtonsEntering = 1;
			Root.SelectPen(Root.CurrentPen);

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}

		public void btStop_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			RetreatAndExit();
		}

		DateTime LastTickTime;
		bool[] LastPenStatus = new bool[10];
		bool LastEraserStatus = false;
		bool LastVisibleStatus = false;
		bool LastPointerStatus = false;
		bool LastPanStatus = false;
		bool LastUndoStatus = false;
		bool LastRedoStatus = false;
		bool LastSnapStatus = false;
		bool LastClearStatus = false;

        // Added  by mwtoolz- Hotkeys       
		// ===
        bool LastLineStatus = false;
        bool LastDottedLineStatus = false;
        bool LastArrowLineStatus = false;
        bool LastRectangleStatus = false;
        bool LastFilledRectangleStatus = false;
        bool LastCircleStatus = false;
        bool LastFilledCircleStatus = false;


		//===

        private void gpPenWidth_MouseDown(object sender, MouseEventArgs e)
		{
			gpPenWidth_MouseOn = true;
		}

		private void gpPenWidth_MouseMove(object sender, MouseEventArgs e)
		{
			if (gpPenWidth_MouseOn)
			{
				if (e.X < 10 || gpPenWidth.Width - e.X < 10)
					return;

				Root.GlobalPenWidth = e.X * e.X / 30;
				pboxPenWidthIndicator.Left = e.X - pboxPenWidthIndicator.Width / 2;
				IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;
				Root.UponButtonsUpdate |= 0x2;
			}
		}

		private void gpPenWidth_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.X >= 10 && gpPenWidth.Width - e.X >= 10)
			{
				Root.GlobalPenWidth = e.X * e.X / 30;
				pboxPenWidthIndicator.Left = e.X - pboxPenWidthIndicator.Width / 2;
				IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;
			}

			if (Root.CanvasCursor == 1)
				SetPenTipCursor();

			Root.gpPenWidthVisible = false;
			Root.UponSubPanelUpdate = true;
			gpPenWidth_MouseOn = false;
		}

		private void pboxPenWidthIndicator_MouseDown(object sender, MouseEventArgs e)
		{
			gpPenWidth_MouseOn = true;
		}

		private void pboxPenWidthIndicator_MouseMove(object sender, MouseEventArgs e)
		{
			if (gpPenWidth_MouseOn)
			{
				int x = e.X + pboxPenWidthIndicator.Left;
				if (x < 10 || gpPenWidth.Width - x < 10)
					return;

				Root.GlobalPenWidth = x * x / 30;
				pboxPenWidthIndicator.Left = x - pboxPenWidthIndicator.Width / 2;
				IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;
				Root.UponButtonsUpdate |= 0x2;
			}
		}

		private void pboxPenWidthIndicator_MouseUp(object sender, MouseEventArgs e)
		{
			if (Root.CanvasCursor == 1)
				SetPenTipCursor();

			Root.gpPenWidthVisible = false;
			Root.UponSubPanelUpdate = true;
			gpPenWidth_MouseOn = false;
		}

		// Added by mwtoolz- Cursor
		private void SetCanvasCursor()
		{
            if (Root.CanvasCursor == 0)
            {
                cursorred = new System.Windows.Forms.Cursor(
                    gInkPlus.Properties.Resources.cursorred.Handle);

                IC.Cursor = cursorred;
            }
            else if (Root.CanvasCursor == 1)
            {
                SetPenTipCursor();
            }
        }

		private void SetPenTipCursor()
		{
			Bitmap bitmaptip = (Bitmap)(gInkPlus.Properties.Resources._null).Clone();
			Graphics g = Graphics.FromImage(bitmaptip);
			DrawingAttributes dda = IC.DefaultDrawingAttributes;
			Brush cbrush;
			Point widt;
			if (!Root.EraserMode)
			{
				cbrush = new SolidBrush(IC.DefaultDrawingAttributes.Color);
				//Brush cbrush = new SolidBrush(Color.FromArgb(255 - dda.Transparency, dda.Color.R, dda.Color.G, dda.Color.B));
				widt = new Point((int)IC.DefaultDrawingAttributes.Width, 0);
			}
			else
			{
				cbrush = new SolidBrush(Color.Black);
				widt = new Point(60, 0);
			}
			IC.Renderer.InkSpaceToPixel(IC.Handle, ref widt);

			IntPtr screenDc = GetDC(IntPtr.Zero);
			const int VERTRES = 10;
			const int DESKTOPVERTRES = 117;
			int LogicalScreenHeight = GetDeviceCaps(screenDc, VERTRES);
			int PhysicalScreenHeight = GetDeviceCaps(screenDc, DESKTOPVERTRES);
			float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
			ReleaseDC(IntPtr.Zero, screenDc);

			int dia = Math.Max((int)(widt.X * ScreenScalingFactor), 2);
			g.FillEllipse(cbrush, 64 - dia / 2, 64 - dia / 2, dia, dia);
			if (dia <= 5)
			{
				Pen cpen = new Pen(Color.FromArgb(50, 128, 128, 128), 2);
				dia += 6;
				g.DrawEllipse(cpen, 64 - dia / 2, 64 - dia / 2, dia, dia);
			}
			IC.Cursor = new System.Windows.Forms.Cursor(bitmaptip.GetHicon());
			
		}

		short LastESCStatus = 0;
		private void tiSlide_Tick(object sender, EventArgs e)
		{
			// ignore the first tick
			if (LastTickTime.Year == 1987)
			{
				LastTickTime = DateTime.Now;
				return;
			}

			int aimedleft = gpButtonsLeft;
			if (ButtonsEntering == -9)
			{
				aimedleft = gpButtonsLeft + gpButtonsWidth;
			}
			else if (ButtonsEntering < 0)
			{
				if (Root.Snapping > 0)
					aimedleft = gpButtonsLeft + gpButtonsWidth + 0;
				else if (Root.Docked)
					aimedleft = gpButtonsLeft + gpButtonsWidth - btDock.Right;
			}
			else if (ButtonsEntering > 0)
			{
				if (Root.Docked)
					aimedleft = gpButtonsLeft + gpButtonsWidth - btDock.Right;
				else
					aimedleft = gpButtonsLeft;
			}
			else if (ButtonsEntering == 0)
			{
				aimedleft = gpButtons.Left; // stay at current location
			}

			if (gpButtons.Left > aimedleft)
			{
				float dleft = gpButtons.Left - aimedleft;
				dleft /= 70;
				if (dleft > 8) dleft = 8;
				dleft *= (float)(DateTime.Now - LastTickTime).TotalMilliseconds;
				if (dleft > 120) dleft = 230;
				if (dleft < 1) dleft = 1;
				gpButtons.Left -= (int)dleft;
				LastTickTime = DateTime.Now;
				if (gpButtons.Left < aimedleft)
				{
					gpButtons.Left = aimedleft;
				}
				gpButtons.Width = Math.Max(gpButtonsWidth - (gpButtons.Left - gpButtonsLeft), btDock.Width);
				Root.UponButtonsUpdate |= 0x1;
			}
			else if (gpButtons.Left < aimedleft)
			{
				float dleft = aimedleft - gpButtons.Left;
				dleft /= 70;
				if (dleft > 8) dleft = 8;
				// fast exiting when not docked
				if (ButtonsEntering == -9 && !Root.Docked)
					dleft = 8;
				dleft *= (float)(DateTime.Now - LastTickTime).TotalMilliseconds;
				if (dleft > 120) dleft = 120;
				if (dleft < 1) dleft = 1;
				// fast exiting when docked
				if (ButtonsEntering == -9 && dleft == 1)
					dleft = 2;
				gpButtons.Left += (int)dleft;
				LastTickTime = DateTime.Now;
				if (gpButtons.Left > aimedleft)
				{
					gpButtons.Left = aimedleft;
				}
				gpButtons.Width = Math.Max(gpButtonsWidth - (gpButtons.Left - gpButtonsLeft), btDock.Width);
				Root.UponButtonsUpdate |= 0x1;
				Root.UponButtonsUpdate |= 0x4;
			}

			if (ButtonsEntering == -9 && gpButtons.Left == aimedleft)
			{
				tiSlide.Enabled = false;
				Root.StopInk();
				return;
			}
			else if (ButtonsEntering < 0)
			{
				Root.UponAllDrawingUpdate = true;
				Root.UponButtonsUpdate = 0;
			}
			if (gpButtons.Left == aimedleft)
			{
				ButtonsEntering = 0;
			}



			if (!Root.PointerMode && !this.TopMost)
				ToTopMost();

			// gpPenWidth status

			if (Root.gpPenWidthVisible != gpPenWidth.Visible)
				gpPenWidth.Visible = Root.gpPenWidthVisible;

			// hotkeys

			const int VK_LCONTROL = 0xA2;
			const int VK_RCONTROL = 0xA3;
			const int VK_LSHIFT = 0xA0;
			const int VK_RSHIFT = 0xA1;
			const int VK_LMENU = 0xA4;
			const int VK_RMENU = 0xA5;
			const int VK_LWIN = 0x5B;
			const int VK_RWIN = 0x5C;
			bool pressed;

			if (!Root.PointerMode)
			{
				// ESC key : Exit
				short retVal;
				retVal = GetKeyState(27);
				if ((retVal & 0x8000) == 0x8000 && (LastESCStatus & 0x8000) == 0x0000)
				{
					if (Root.Snapping > 0)
					{
						ExitSnapping();
					}
					else if (Root.gpPenWidthVisible)
					{
						Root.gpPenWidthVisible = false;
						Root.UponSubPanelUpdate = true;
					}
					else if (Root.Snapping == 0)
						RetreatAndExit();
				}
				LastESCStatus = retVal;
			}


			if (!Root.FingerInAction && (!Root.PointerMode || Root.AllowHotkeyInPointerMode) && Root.Snapping <= 0)
			{
				bool control = ((short)(GetKeyState(VK_LCONTROL) | GetKeyState(VK_RCONTROL)) & 0x8000) == 0x8000;
				bool alt = ((short)(GetKeyState(VK_LMENU) | GetKeyState(VK_RMENU)) & 0x8000) == 0x8000;
				bool shift = ((short)(GetKeyState(VK_LSHIFT) | GetKeyState(VK_RSHIFT)) & 0x8000) == 0x8000;
				bool win = ((short)(GetKeyState(VK_LWIN) | GetKeyState(VK_RWIN)) & 0x8000) == 0x8000;

				for (int p = 0; p < Root.MaxPenCount; p++)
				{
					pressed = (GetKeyState(Root.Hotkey_Pens[p].Key) & 0x8000) == 0x8000;
					if(pressed && !LastPenStatus[p] && Root.Hotkey_Pens[p].ModifierMatch(control, alt, shift, win))
					{
						SelectPen(p);
					}
					LastPenStatus[p] = pressed;
				}

				pressed = (GetKeyState(Root.Hotkey_Eraser.Key) & 0x8000) == 0x8000;
				if (pressed && !LastEraserStatus && Root.Hotkey_Eraser.ModifierMatch(control, alt, shift, win))
				{
					SelectPen(-1);
				}
				LastEraserStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_InkVisible.Key) & 0x8000) == 0x8000;
				if (pressed && !LastVisibleStatus && Root.Hotkey_InkVisible.ModifierMatch(control, alt, shift, win))
				{
					btInkVisible_Click(null, null);
				}
				LastVisibleStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Undo.Key) & 0x8000) == 0x8000;
				if (pressed && !LastUndoStatus && Root.Hotkey_Undo.ModifierMatch(control, alt, shift, win))
				{
					if (!Root.InkVisible)
						Root.SetInkVisible(true);

					Root.UndoInk();
				}
				LastUndoStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Redo.Key) & 0x8000) == 0x8000;
				if (pressed && !LastRedoStatus && Root.Hotkey_Redo.ModifierMatch(control, alt, shift, win))
				{
					Root.RedoInk();
				}
				LastRedoStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Pointer.Key) & 0x8000) == 0x8000;
				if (pressed && !LastPointerStatus && Root.Hotkey_Pointer.ModifierMatch(control, alt, shift, win))
				{
					SelectPen(-2);
				}
				LastPointerStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Pan.Key) & 0x8000) == 0x8000;
				if (pressed && !LastPanStatus && Root.Hotkey_Pan.ModifierMatch(control, alt, shift, win))
				{
					SelectPen(-3);
				}
				LastPanStatus = pressed;

                // Added by mwtoolz- Hotkeys
                // ===
				// Line
                pressed = (GetKeyState(Root.Hotkey_Line.Key) & 0x8000) == 0x8000;
                if (pressed && !LastLineStatus && Root.Hotkey_Line.ModifierMatch(control, alt, shift, win))
                {
                    SelectPen(-4);
				}
				LastLineStatus = pressed;
                // Rectangle
                pressed = (GetKeyState(Root.Hotkey_Rectangle.Key) & 0x8000) == 0x8000;
                if (pressed && !LastRectangleStatus &&
                    Root.Hotkey_Rectangle.ModifierMatch(control, alt, shift, win))
                {
                    SelectPen(-5);
                }
                LastRectangleStatus = pressed;
                // Filled Rectangle
                pressed = (GetKeyState(Root.Hotkey_Filled_Rectangle.Key) & 0x8000) == 0x8000;
                if (pressed && !LastFilledRectangleStatus &&
                    Root.Hotkey_Filled_Rectangle.ModifierMatch(control, alt, shift, win))
                {
                    SelectPen(-6);
                }
                LastFilledRectangleStatus = pressed;
                // Dotted Line
                pressed = (GetKeyState(Root.Hotkey_Dotted_Line.Key) & 0x8000) == 0x8000;
                if (pressed && !LastDottedLineStatus &&
                    Root.Hotkey_Dotted_Line.ModifierMatch(control, alt, shift, win))
                {
                    SelectPen(-7); // Confirm the mode value
                }
                LastDottedLineStatus = pressed;

                // Arrow Line
                pressed = (GetKeyState(Root.Hotkey_Arrow_Line.Key) & 0x8000) == 0x8000;
                if (pressed && !LastArrowLineStatus &&
                    Root.Hotkey_Arrow_Line.ModifierMatch(control, alt, shift, win))
                {
                    SelectPen(-8); // Confirm the mode value
                }
                LastArrowLineStatus = pressed;




                //===


                pressed = (GetKeyState(Root.Hotkey_Clear.Key) & 0x8000) == 0x8000;
				if (pressed && !LastClearStatus && Root.Hotkey_Clear.ModifierMatch(control, alt, shift, win))
				{
					btClear_Click(null, null);
				}
				LastClearStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Snap.Key) & 0x8000) == 0x8000;
				if (pressed && !LastSnapStatus && Root.Hotkey_Snap.ModifierMatch(control, alt, shift, win))
				{
					btSnap_Click(null, null);
				}
				LastSnapStatus = pressed;
			}

			if (Root.Snapping < 0)
				Root.Snapping++;
		}

		private bool IsInsideVisibleScreen(int x, int y)
		{
			x -= PrimaryLeft;
			y -= PrimaryTop;
			//foreach (Screen s in Screen.AllScreens)
			//	Console.WriteLine(s.Bounds);
			//Console.WriteLine(x.ToString() + ", " + y.ToString());

			foreach (Screen s in Screen.AllScreens)
				if (s.Bounds.Contains(x, y))
					return true;
			return false;
		}

		int IsMovingToolbar = 0;
		Point HitMovingToolbareXY = new Point();
		bool ToolbarMoved = false;
		private void gpButtons_MouseDown(object sender, MouseEventArgs e)
		{
			if (!Root.AllowDraggingToolbar)
				return;
			if (ButtonsEntering != 0)
				return;

			ToolbarMoved = false;
			IsMovingToolbar = 1;
			HitMovingToolbareXY.X = e.X;
			HitMovingToolbareXY.Y = e.Y;
		}

		private void gpButtons_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsMovingToolbar == 1)
			{
				if (Math.Abs(e.X - HitMovingToolbareXY.X) > 20 || Math.Abs(e.Y - HitMovingToolbareXY.Y) > 20)
					IsMovingToolbar = 2;
			}
			if (IsMovingToolbar == 2)
			{
				if (e.X != HitMovingToolbareXY.X || e.Y != HitMovingToolbareXY.Y)
				{
					/*
					gpButtonsLeft += e.X - HitMovingToolbareXY.X;
					gpButtonsTop += e.Y - HitMovingToolbareXY.Y;
					
					if (gpButtonsLeft + gpButtonsWidth > SystemInformation.VirtualScreen.Right)
						gpButtonsLeft = SystemInformation.VirtualScreen.Right - gpButtonsWidth;
					if (gpButtonsLeft < SystemInformation.VirtualScreen.Left)
						gpButtonsLeft = SystemInformation.VirtualScreen.Left;
					if (gpButtonsTop + gpButtonsHeight > SystemInformation.VirtualScreen.Bottom)
						gpButtonsTop = SystemInformation.VirtualScreen.Bottom - gpButtonsHeight;
					if (gpButtonsTop < SystemInformation.VirtualScreen.Top)
						gpButtonsTop = SystemInformation.VirtualScreen.Top;
					*/
					int newleft = gpButtonsLeft + e.X - HitMovingToolbareXY.X;
					int newtop = gpButtonsTop + e.Y - HitMovingToolbareXY.Y;

					bool continuemoving;
					bool toolbarmovedthisframe = false;
					int dleft = 0, dtop = 0;
					if
					(
						IsInsideVisibleScreen(newleft, newtop) &&
						IsInsideVisibleScreen(newleft + gpButtonsWidth, newtop) &&
						IsInsideVisibleScreen(newleft, newtop + gpButtonsHeight) &&
						IsInsideVisibleScreen(newleft + gpButtonsWidth, newtop + gpButtonsHeight)
					)
					{
						continuemoving = true;
						ToolbarMoved = true;
						toolbarmovedthisframe = true;
						dleft = newleft - gpButtonsLeft;
						dtop = newtop - gpButtonsTop;
					}
					else
					{
						do
						{
							if (dleft != newleft - gpButtonsLeft)
								dleft += Math.Sign(newleft - gpButtonsLeft);
							else
								break;
							if
							(
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + gpButtonsHeight + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + gpButtonsHeight + dtop)
							)
							{
								continuemoving = true;
								ToolbarMoved = true;
								toolbarmovedthisframe = true;
							}
							else
							{
								continuemoving = false;
								dleft -= Math.Sign(newleft - gpButtonsLeft);
							}
						}
						while (continuemoving);
						do
						{
							if (dtop != newtop - gpButtonsTop)
								dtop += Math.Sign(newtop - gpButtonsTop);
							else
								break;
							if
							(
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + gpButtonsHeight + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + gpButtonsHeight + dtop)
							)
							{
								continuemoving = true;
								ToolbarMoved = true;
								toolbarmovedthisframe = true;
							}
							else
							{
								continuemoving = false;
								dtop -= Math.Sign(newtop - gpButtonsTop);
							}
						}
						while (continuemoving);
					}

					if (toolbarmovedthisframe)
					{
						gpButtonsLeft += dleft;
						gpButtonsTop += dtop;
						Root.gpButtonsLeft = gpButtonsLeft;
						Root.gpButtonsTop = gpButtonsTop;
						if (Root.Docked)
							gpButtons.Left = gpButtonsLeft + gpButtonsWidth - btDock.Right;
						else
							gpButtons.Left = gpButtonsLeft;
						gpPenWidth.Left = gpButtonsLeft + btPenWidth.Left - gpPenWidth.Width / 2 + btPenWidth.Width / 2;
						gpPenWidth.Top = gpButtonsTop - gpPenWidth.Height - 10;
						gpButtons.Top = gpButtonsTop;
						Root.UponAllDrawingUpdate = true;
					}
				}
			}
		}

		private void gpButtons_MouseUp(object sender, MouseEventArgs e)
		{
			IsMovingToolbar = 0;
		}

		private void btInkVisible_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			Root.SetInkVisible(!Root.InkVisible);
		}

		public void btClear_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			Root.ClearInk();
			SaveUndoStrokes();
		}

		private void btUndo_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			if (!Root.InkVisible)
				Root.SetInkVisible(true);

			Root.UndoInk();
		}

		public void btColor_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			for (int b = 0; b < Root.MaxPenCount; b++)
				if ((Button)sender == btPen[b])
				{
					SelectPen(b);
				}
		}

		public void btEraser_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			SelectPen(-1);
		}


		private void btPan_Click(object sender, EventArgs e)
		{
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			SelectPen(-3);
		}

		short LastF4Status = 0;
		private void FormCollection_FormClosing(object sender, FormClosingEventArgs e)
		{
			// check if F4 key is pressed and we assume it's Alt+F4
			short retVal = GetKeyState(0x73);
			if ((retVal & 0x8000) == 0x8000 && (LastF4Status & 0x8000) == 0x0000)
			{
				e.Cancel = true;

				// the following block is copyed from tiSlide_Tick() where we check whether ESC is pressed
				if (Root.Snapping > 0)
				{
					ExitSnapping();
				}
				else if (Root.gpPenWidthVisible)
				{
					Root.gpPenWidthVisible = false;
					Root.UponSubPanelUpdate = true;
				}
				else if (Root.Snapping == 0)
					RetreatAndExit();
			}

			LastF4Status = retVal;       

        }

        // Added by mwtoolz for FormCollection memory cleanup
        // ===
        private void FormCollection_FormClosed(object sender, FormClosedEventArgs e)
        {

            // Unsubscribe InkOverlay events
            if (IC != null)
            {
                IC.CursorInRange -= IC_CursorInRange;
				IC.MouseDown -= IC_MouseDown;
                IC.MouseMove -= IC_MouseMove;
                IC.MouseUp -= IC_MouseUp;
                IC.CursorDown -= IC_CursorDown;
                IC.Stroke -= IC_Stroke;

                // Dispose InkOverlay
                IC.Dispose();
                IC = null;              
            }      

            // Dispose toolbar images
            image_exit?.Dispose();
            image_clear?.Dispose();
            image_undo?.Dispose();
            image_snap?.Dispose();
            image_penwidth?.Dispose();

            image_dock?.Dispose();
            image_dockback?.Dispose();

            image_pencil?.Dispose();
            image_highlighter?.Dispose();
            image_pencil_act?.Dispose();
            image_highlighter_act?.Dispose();

            image_pointer?.Dispose();
            image_pointer_act?.Dispose();

            image_eraser?.Dispose();
            image_eraser_act?.Dispose();

            image_pan?.Dispose();
            image_pan_act?.Dispose();

            image_visible?.Dispose();
            image_visible_not?.Dispose();

            // Dispose custom gInkPlus tool images
            image_line?.Dispose();
            image_line_act?.Dispose();

            image_rect?.Dispose();
            image_rect_act?.Dispose();

            image_rect_filled?.Dispose();
            image_rect_filled_act?.Dispose();        

            // Dispose custom cursor
            cursorred?.Dispose();
            cursorred = null;
            cursorsnap?.Dispose();
			cursorsnap = null;
            cursortip?.Dispose();
			cursortip = null;            
        }

		// ===



        // Reset all custom shape tools
        private void ResetShapeToolsMode()
        {
            // Line
            Root.FormDisplay.ResetLineDrawingState();
            Root.LineMode = false;
            btLine.Image = image_line;

            // Rectangle
            Root.FormDisplay.ResetRectDrawingState();
            Root.RectMode = false;
            btRect.Image = image_rect;

            // Filled Rectangle
            Root.FormDisplay.ResetRectFilledDrawingState();
            Root.RectFilledMode = false;
            btRectFilled.Image = image_rect_filled;
        }

        // Reset general toolbar buttons and modes
        private void ResetGeneralToolsMode()
        {
            // Restore all pen buttons
            for (int b = 0; b < Root.MaxPenCount; b++)
                btPen[b].Image = image_pen[b];

            // Restore other toolbar buttons
            btEraser.Image = image_eraser;
            btPointer.Image = image_pointer;
            btPan.Image = image_pan;

            // Deactivate other modes
            EnterEraserMode(false);
            Root.UnPointer();
            Root.PanMode = false;
			
        }

        // ===

        // Added by mwtoolz
        // Keep full input area so mouse events are received across the entire form     
        private void SetFullInputArea()
        {
            SetInkInputRectangle(new Rectangle(0, 0, Width, Height));
        }


        // Added by mwtoolz
        private void SetInkInputRectangle(Rectangle rect)
        {
            if (IsDisposed || Disposing || !IsHandleCreated || IC == null)
                return;

            if (IC.CollectingInk)
			{             
                return;
            }

            try
            {
                IC.SetWindowInputRectangle(rect);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "SetInkInputRectangle failed: " + ex.Message);
            }			         
        }


        [DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		[DllImport("user32.dll", SetLastError = true)]
		static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);
		[DllImport("user32.dll")]
		public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
		[DllImport("user32.dll", SetLastError = false)]
		static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern short GetKeyState(int keyCode);

		[DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		[DllImport("user32.dll")]
		static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
	}
}
