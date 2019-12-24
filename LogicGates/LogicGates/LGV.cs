using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogicGates
{
    public partial class LGV : Form
    {
        Bitmap bCanvas;
        Graphics gCanvas;
        Vector vOffSet;
        Vector vStartPan;
        List<LogicGate> logicgates;
        List<Wire> wires;
        LogicGate lgSelected;
        Connection cSelected;
        float fScale;
        int nSelected;
        bool bDerived;
        Wire wDerived;
        bool bHelpMenu;
        string sHelpText;
        Vector vHelpPos;
        Font HelpFont;
        public LGV()
        {
            InitializeComponent();
            CenterToScreen();
            fScale = 1f;
            bCanvas = new Bitmap(Canvas.Width, Canvas.Height);
            gCanvas = Graphics.FromImage(bCanvas);
            gCanvas.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            sHelpText = "- Right-Mouse to select and drag one or more gates.\n" +
                        "- Left-Mouse to bind/unbind gates or switch input.\n" +
                        "- Middle-Mouse to move the world.\n" +
                        "- Delete to delete selected gates.\n" +
                        "- I to place a IN gate.\n" +
                        "- O to place a OUT gate.\n" +
                        "- 1 to place a NOT gate.\n" +
                        "- 2 to place a AND gate.\n" +
                        "- 3 to place a OR gate.\n" +
                        "- 4 to place a NAND gate.\n" +
                        "- 5 to place a NOR gate.\n" +
                        "- 6 to place a XOR gate.\n" +
                        "- 7 to place a XNOR(XAND) gate";
            HelpFont = new Font("Microsoft Sans Serif", 15f);
            SizeF size = gCanvas.MeasureString(sHelpText, HelpFont);
            vHelpPos = new Vector(HelpButton.Location.X - size.Width, HelpButton.Location.Y);
            vOffSet = new Vector(-bCanvas.Width * 0.5f, -bCanvas.Height * 0.5f);
            vStartPan = new Vector();
            logicgates = new List<LogicGate>();
            wires = new List<Wire>();
            logicgates.Add(new AND(0, 0));
            logicgates.Add(new IN(-300, 100));
            logicgates.Add(new IN(-300, -100));
            logicgates.Add(new OUT(300, 0));
            Input.LinkReferences(this, Canvas);
        }
        private Vector WorldToScreen(Vector v)
        {
            return (v - vOffSet) * fScale;
        }
        private Vector ScreenToWorld(Vector v)
        {
            return (v / fScale) + vOffSet;
        }
        private void Update(object sender, EventArgs e)
        {
            gCanvas.Clear(Color.FromArgb(80,50,40));
            Vector vMouse = Input.MousePos();
            Vector vMouseBZ = ScreenToWorld(vMouse);
            if (Input.WheelScrollUp()) fScale *= 1.1f;
            if (Input.WheelScrollDown()) fScale *= 0.9f;
            Vector vMouseAZ = ScreenToWorld(vMouse);
            vOffSet += vMouseBZ - vMouseAZ;

            if (Input.KeyPressed(MouseButtons.Middle))
            {
                vStartPan = new Vector(vMouse.x, vMouse.y);
            }
            if (Input.KeyHeld(MouseButtons.Middle))
            {
                vOffSet -= (vMouse - vStartPan) / fScale;
                vStartPan = new Vector(vMouse.x, vMouse.y);
            }

            if (Input.KeyPressed(MouseButtons.Right))
            {
                if (nSelected == 0)
                {
                    foreach (LogicGate lg in logicgates)
                    {
                        Vector v1 = WorldToScreen(new Vector(lg.vWorldPos.x - lg.nWidth * 0.5f, lg.vWorldPos.y - lg.nHeight * 0.5f));
                        Vector v2 = WorldToScreen(new Vector(lg.vWorldPos.x + lg.nWidth * 0.5f, lg.vWorldPos.y + lg.nHeight * 0.5f));
                        if (!vMouse.OutsideBounds(v1.x, v1.y, v2.x, v2.y))
                        {
                            lgSelected = lg;
                            lgSelected.bSelected = true;
                            break;
                        }
                    }
                }
                else
                {
                    bool none = true;
                    foreach (LogicGate lg in logicgates)
                    {
                        Vector v1 = WorldToScreen(new Vector(lg.vWorldPos.x - lg.nWidth * 0.5f, lg.vWorldPos.y - lg.nHeight * 0.5f));
                        Vector v2 = WorldToScreen(new Vector(lg.vWorldPos.x + lg.nWidth * 0.5f, lg.vWorldPos.y + lg.nHeight * 0.5f));
                        if (!vMouse.OutsideBounds(v1.x, v1.y, v2.x, v2.y))
                        {
                            if (lg.bSelected) none = false;
                            else lgSelected = lg;
                            break;
                        }
                    }
                    if (none)
                    {
                        nSelected = 0;
                        foreach (LogicGate lg in logicgates) lg.bSelected = false;
                        if (lgSelected != null) lgSelected.bSelected = true;
                    }
                }
                vStartPan = new Vector(vMouse.x, vMouse.y);
            }
            if (Input.KeyHeld(MouseButtons.Right))
            {
                if (nSelected == 0)
                {
                    if (lgSelected != null)
                    {
                        lgSelected.vWorldPos += (vMouse - vStartPan) / fScale;
                        vStartPan = new Vector(vMouse.x, vMouse.y);
                    }
                    else
                    {
                        gCanvas.DrawLine(Pens.Green, vStartPan.x, vStartPan.y, vMouse.x, vStartPan.y);
                        gCanvas.DrawLine(Pens.Green, vStartPan.x, vStartPan.y, vStartPan.x, vMouse.y);
                        gCanvas.DrawLine(Pens.Green, vMouse.x, vStartPan.y, vMouse.x, vMouse.y);
                        gCanvas.DrawLine(Pens.Green, vStartPan.x, vMouse.y, vMouse.x, vMouse.y);
                    }
                }
                else
                {
                    foreach (LogicGate lg in logicgates)
                        if (lg.bSelected) lg.vWorldPos += (vMouse - vStartPan) / fScale;
                    vStartPan = new Vector(vMouse.x, vMouse.y);
                }
            }
            if (Input.KeyReleased(MouseButtons.Right))
            {
                if (lgSelected != null)
                {
                    lgSelected.bSelected = false;
                    lgSelected = null;
                }
                foreach (LogicGate lg in logicgates)
                {
                    Vector mouseworld = ScreenToWorld(vMouse);
                    Vector startpanworld = ScreenToWorld(vStartPan);
                    if (!lg.vWorldPos.OutsideBounds(mouseworld.x, mouseworld.y, startpanworld.x, startpanworld.y))
                    {
                        lg.bSelected = true;
                        nSelected++;
                    }
                }
            }

            if (Input.KeyPressed(MouseButtons.Left))
            {
                foreach (LogicGate lg in logicgates)
                {
                    foreach (Connection c in lg.cConnections)
                    {
                        if ((c.vScreenPos - vMouse).Magnitude() < c.nSize * fScale * 0.5f) 
                        {
                            lgSelected = lg;
                            cSelected = c;
                            goto Next;
                        }
                    }
                }
                foreach (Wire w in wires)
                {
                    if (DistancePointToLine(vMouse, w) <= 10f * fScale * 0.5f) 
                    {
                        lgSelected = w.lgDonor;
                        cSelected = w.cDonor;
                        bDerived = true;
                        wDerived = w;
                        goto Next;
                    }
                }
            Next:
                vStartPan = new Vector(vMouse.x, vMouse.y);
            }
            if (Input.KeyHeld(MouseButtons.Left))
            {
                if (cSelected != null)
                    gCanvas.DrawLine(new Pen(cSelected.bState ? Color.Green : Color.Red, 10f * fScale), 
                                     vStartPan.AsPoint(), vMouse.AsPoint());
            }
            if (Input.KeyReleased(MouseButtons.Left))
            {
                if (cSelected != null)
                {
                    foreach (LogicGate lg in logicgates)
                    {
                        foreach (Connection c in lg.cConnections)
                        {
                            if ((c.vScreenPos - vMouse).Magnitude() < c.nSize * fScale * 0.5f &&
                                c.bAsInput != cSelected.bAsInput && lg != lgSelected && !c.bConnected && !cSelected.bConnected)
                            {
                                Wire w = new Wire(lgSelected, lg, cSelected, c);
                                c.bConnected = true;
                                c.wLinkWire = w;
                                cSelected.bConnected = true;
                                cSelected.wLinkWire = w;
                                wires.Add(w);
                                goto Next;
                            }
                            if ((c.vScreenPos - vMouse).Magnitude() < c.nSize * fScale * 0.5f &&
                                c.bAsInput != cSelected.bAsInput && lg != lgSelected && bDerived)
                            {
                                Wire w = new Wire(lgSelected, lg, cSelected, c);
                                wDerived.derivedwires.Add(w);
                                w.bDerivedWire = true;
                                w.vDerivedPin = PointOnLineClosest(vStartPan, wDerived);
                                w.wParentWire = wDerived;
                                w.Calculate();
                                c.bConnected = true;
                                c.wLinkWire = w;
                                wires.Add(w);
                                goto Next;
                            }
                            if ((c.vScreenPos - vMouse).Magnitude() < c.nSize * fScale * 0.5f &&
                                lg == lgSelected && lg.eGateType == GateType.IN)
                            {
                                lg.bState = !lg.bState;
                                goto Next;
                            }
                        }
                    }
                    for (int i = 0; i < wires.Count; i++)
                    {
                        if (wires[i] == cSelected.wLinkWire && !bDerived)
                        {
                            if (!wires[i].bDerivedWire)
                            {
                                wires[i].cDonor.wLinkWire = null;
                                wires[i].cReceiver.wLinkWire = null;
                                wires[i].cDonor.bConnected = false;
                                wires[i].cReceiver.bConnected = false;
                                wires[i].cDonor.bState = false;
                                wires[i].cReceiver.bState = false;
                            }
                            else
                            {
                                wires[i].cReceiver.wLinkWire = null;
                                wires[i].cReceiver.bConnected = false;
                                wires[i].cReceiver.bState = false;
                            }
                            DeleteDerivedWires(wires[i]);
                            wires.RemoveAt(i);
                            goto Next;
                        }
                    }
                Next:
                    bDerived = false;
                    wDerived = null;
                    lgSelected = null;
                    cSelected = null;
                }
            }

            if (Input.KeyPressed(Keys.I))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new IN(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.O))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new OUT(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.D1))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new NOT(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.D2))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new AND(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.D3))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new OR(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.D4))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new NAND(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.D5))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new NOR(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.D6))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new XOR(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.D7))
            {
                Vector v = ScreenToWorld(vMouse);
                logicgates.Add(new XNOR(v.x, v.y));
            }
            if (Input.KeyPressed(Keys.Delete) && nSelected > 0)
            {
                while (nSelected > 0)
                {
                    for (int i = 0; i < logicgates.Count; i++)
                    {
                        if (logicgates[i].bSelected)
                        {
                            for (int j = 0; j < logicgates[i].cConnections.Length; j++)
                            {
                                Wire w = logicgates[i].cConnections[j].wLinkWire;
                                if (w != null)
                                {
                                    w.cDonor.wLinkWire = null;
                                    w.cReceiver.wLinkWire = null;
                                    w.cDonor.bConnected = false;
                                    w.cReceiver.bConnected = false;
                                    w.cDonor.bState = false;
                                    w.cReceiver.bState = false;
                                }
                                wires.Remove(w);
                            }
                            logicgates.RemoveAt(i);
                            nSelected--;
                        }
                    }
                }
            }

            Vector coordonates = ScreenToWorld(vMouse);
            foreach (LogicGate lg in logicgates)
                lg.Render(ref gCanvas, vOffSet, fScale);
            foreach (Wire w in wires)
                w.Render(ref gCanvas, fScale);
            gCanvas.DrawString("Zoom Level: " + (int)(fScale * 100) + "%\n" + 
                               "World coordonates: " + (int)coordonates.x + " " + -(int)coordonates.y, 
                               new Font("Times New Roman", 15f), Brushes.Black, 0, 0);
            if (bHelpMenu)
                gCanvas.DrawString(sHelpText, HelpFont, Brushes.Black, vHelpPos.AsPoint());
            Canvas.Image = bCanvas;
            Input.UpdateKeys();
        }
        private float DistancePointToLine(Vector v, Wire w)
        {
            float x0 = v.x;
            float y0 = v.y;
            float x1 = w.bDerivedWire ? w.vDerivedPin.x : w.cDonor.vScreenPos.x;
            float y1 = w.bDerivedWire ? w.vDerivedPin.y : w.cDonor.vScreenPos.y;
            float x2 = w.cReceiver.vScreenPos.x;
            float y2 = w.cReceiver.vScreenPos.y;
            float up = Math.Abs((y2 - y1) * x0 - (x2 - x1) * y0 + x2 * y1 - y2 * x1);
            float down = (float)Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
            return up / down;
        }
        private Vector PointOnLineClosest(Vector v, Wire w)
        {
            float x0 = v.x;
            float y0 = v.y;
            float x1 = w.bDerivedWire ? w.vDerivedPin.x : w.cDonor.vScreenPos.x;
            float y1 = w.bDerivedWire ? w.vDerivedPin.y : w.cDonor.vScreenPos.y;
            float x2 = w.cReceiver.vScreenPos.x;
            float y2 = w.cReceiver.vScreenPos.y;
            float m = (y1 - y2) / (x1 - x2);
            float k = y1 - m * x1;
            float x = (x0 + m * y0 - m * k) / (m * m + 1);
            float y = m * x + k;
            return new Vector(x, y);
        }
        private void DeleteDerivedWires(Wire wire)
        {
            if (wire.derivedwires.Count > 0)
            {
                foreach (Wire w in wire.derivedwires)
                {
                    DeleteDerivedWires(w);
                    w.cReceiver.wLinkWire = null;
                    w.cReceiver.bConnected = false;
                    w.cReceiver.bState = false;
                    wires.Remove(w);
                }
            }
        }
        private void HelpButton_Click(object sender, EventArgs e)
        {
            bHelpMenu = !bHelpMenu;
        }
    }
}
