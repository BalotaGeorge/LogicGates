using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicGates
{
    enum GateType
    {
        NOT,
        OR,
        AND,
        NOR,
        NAND,
        XOR,
        XNOR,
        IN,
        OUT
    }
    abstract class LogicGate
    {
        public GateType eGateType;
        public Bitmap bInitialImage;
        public Bitmap bProcessedImage;
        public Vector vWorldPos;
        public Vector vScreenPos;
        public Connection[] cConnections;
        public int nWidth;
        public int nHeight;
        public float fLastScale;
        public bool bSelected;
        public bool bState;
        public LogicGate(GateType GateType, float WPosX, float WPosY)
        {
            eGateType = GateType;
            vWorldPos = new Vector(WPosX, WPosY);
            vScreenPos = new Vector();
            switch (eGateType)
            {
                case GateType.NOT:
                    bInitialImage = new Bitmap("../../gates/NOT_gate.png");
                    bProcessedImage = new Bitmap(bInitialImage);
                    nWidth = bInitialImage.Width;
                    nHeight = bInitialImage.Height;
                    break;
                case GateType.AND:
                    bInitialImage = new Bitmap("../../gates/AND_gate.png");
                    bProcessedImage = new Bitmap(bInitialImage);
                    nWidth = bInitialImage.Width;
                    nHeight = bInitialImage.Height;
                    break;
                case GateType.OR:
                    bInitialImage = new Bitmap("../../gates/OR_gate.png");
                    bProcessedImage = new Bitmap(bInitialImage);
                    nWidth = bInitialImage.Width;
                    nHeight = bInitialImage.Height;
                    break;
                case GateType.NAND:
                    bInitialImage = new Bitmap("../../gates/NAND_gate.png");
                    bProcessedImage = new Bitmap(bInitialImage);
                    nWidth = bInitialImage.Width;
                    nHeight = bInitialImage.Height;
                    break;
                case GateType.NOR:
                    bInitialImage = new Bitmap("../../gates/NOR_gate.png");
                    bProcessedImage = new Bitmap(bInitialImage);
                    nWidth = bInitialImage.Width;
                    nHeight = bInitialImage.Height;
                    break;
                case GateType.XOR:
                    bInitialImage = new Bitmap("../../gates/XOR_gate.png");
                    bProcessedImage = new Bitmap(bInitialImage);
                    nWidth = bInitialImage.Width;
                    nHeight = bInitialImage.Height;
                    break;
                case GateType.XNOR:
                    bInitialImage = new Bitmap("../../gates/XNOR_gate.png");
                    bProcessedImage = new Bitmap(bInitialImage);
                    nWidth = bInitialImage.Width;
                    nHeight = bInitialImage.Height;
                    break;
                case GateType.IN:
                    nWidth = nHeight = 40;
                    bInitialImage = new Bitmap(nWidth, nHeight);
                    bProcessedImage = new Bitmap(bInitialImage);
                    break;
                case GateType.OUT:
                    nWidth = nHeight = 40;
                    bInitialImage = new Bitmap(nWidth, nHeight);
                    bProcessedImage = new Bitmap(bInitialImage);
                    break;
            }
        }
        public virtual void Render(ref Graphics g, Vector offset, float Scale)
        {
            if (fLastScale != Scale)
            {
                fLastScale = Scale;
                bProcessedImage = new Bitmap(bInitialImage, (int)(nWidth * fLastScale), (int)(nHeight * fLastScale));
            }
            vScreenPos = WorldToScreen(vWorldPos, offset, Scale);
            g.DrawImage(bProcessedImage, vScreenPos.x - bProcessedImage.Width * 0.5f, vScreenPos.y - bProcessedImage.Height * 0.5f);
            Font font = new Font("Microsoft Sans Serif", 32f * Scale);
            SizeF size = g.MeasureString(eGateType.ToString(), font);
            switch (eGateType)
            {
                case GateType.NOT:
                    g.DrawString(eGateType.ToString(), font, Brushes.Black, vScreenPos.x - size.Width * 0.9f, 
                                                                            vScreenPos.y - size.Height * 0.5f);
                    break;
                case GateType.OR:
                    g.DrawString(eGateType.ToString(), font, Brushes.Black, vScreenPos.x - size.Width * 0.5f, 
                                                                            vScreenPos.y - size.Height * 0.5f);
                    break;
                case GateType.AND:
                    g.DrawString(eGateType.ToString(), font, Brushes.Black, vScreenPos.x - size.Width * 0.5f, 
                                                                            vScreenPos.y - size.Height * 0.5f);
                    break;
                case GateType.NOR:
                    g.DrawString(eGateType.ToString(), font, Brushes.Black, vScreenPos.x - size.Width * 0.6f, 
                                                                            vScreenPos.y - size.Height * 0.5f);
                    break;
                case GateType.NAND:
                    g.DrawString(eGateType.ToString(), font, Brushes.Black, vScreenPos.x - size.Width * 0.6f, 
                                                                            vScreenPos.y - size.Height * 0.5f);
                    break;
                case GateType.XOR:
                    g.DrawString(eGateType.ToString(), font, Brushes.Black, vScreenPos.x - size.Width * 0.4f, 
                                                                            vScreenPos.y - size.Height * 0.5f);
                    break;
                case GateType.XNOR:
                    g.DrawString(eGateType.ToString(), font, Brushes.Black, vScreenPos.x - size.Width * 0.5f, 
                                                                            vScreenPos.y - size.Height * 0.5f);
                    break;
                case GateType.IN: break;
                case GateType.OUT: break;
            }
            if (bSelected)
                g.DrawRectangle(Pens.Green, vScreenPos.x - bProcessedImage.Width * 0.5f, 
                                            vScreenPos.y - bProcessedImage.Height * 0.5f, bProcessedImage.Width, bProcessedImage.Height);
        }
        public Vector WorldToScreen(Vector v, Vector offset, float Scale)
        {
            return (v - offset) * Scale;
        }
    }
    class Wire
    {
        public LogicGate lgDonor;
        public LogicGate lgReceiver;
        public Connection cDonor;
        public Connection cReceiver;
        public List<Wire> derivedwires;
        public Wire wParentWire;
        public Vector vDerivedPin;
        public bool bDerivedWire;
        public bool bState;
        public float t;
        public Wire(LogicGate lgFrom, LogicGate lgTo, Connection cFrom, Connection cTo)
        {
            derivedwires = new List<Wire>();
            lgDonor = lgFrom;
            lgReceiver = lgTo;
            cDonor = cFrom;
            cReceiver = cTo;
        }
        public void Calculate()
        {
            if (wParentWire.vDerivedPin != null)
                t = (vDerivedPin - wParentWire.vDerivedPin).Magnitude() / 
                    (wParentWire.cReceiver.vScreenPos - wParentWire.vDerivedPin).Magnitude();
            else
                t = (vDerivedPin - wParentWire.cDonor.vScreenPos).Magnitude() / 
                    (wParentWire.cReceiver.vScreenPos - wParentWire.cDonor.vScreenPos).Magnitude();
        }
        public void Render(ref Graphics g, float Scale)
        {
            if ((!cDonor.bAsInput && lgDonor.bState) || (!cReceiver.bAsInput && lgReceiver.bState)) bState = true;
            else bState = false;
            cDonor.bState = bState;
            cReceiver.bState = bState;
            if (!bDerivedWire)
            {
                if (bState)
                    g.DrawLine(new Pen(Color.Green, 10 * Scale), cDonor.vScreenPos.AsPoint(), cReceiver.vScreenPos.AsPoint());
                else
                    g.DrawLine(new Pen(Color.Red, 10 * Scale), cDonor.vScreenPos.AsPoint(), cReceiver.vScreenPos.AsPoint());
            }
            else
            {
                if (wParentWire.vDerivedPin != null)
                    vDerivedPin = (wParentWire.cReceiver.vScreenPos - wParentWire.vDerivedPin) * t + wParentWire.vDerivedPin;
                else
                    vDerivedPin = (wParentWire.cReceiver.vScreenPos - wParentWire.cDonor.vScreenPos) * t + wParentWire.cDonor.vScreenPos;
                if (bState)
                    g.DrawLine(new Pen(Color.Green, 10 * Scale), vDerivedPin.AsPoint(), cReceiver.vScreenPos.AsPoint());
                else
                    g.DrawLine(new Pen(Color.Red, 10 * Scale), vDerivedPin.AsPoint(), cReceiver.vScreenPos.AsPoint());
            }
        }
    }
    class Connection
    {
        public Wire wLinkWire;
        public Vector vRelativePos;
        public Vector vScreenPos;
        public int nSize;
        public bool bConnected;
        public bool bAsInput;
        public bool bState;
        public Connection(float RPosX, float RPosY, bool AsInput)
        {
            nSize = 30;
            vRelativePos = new Vector(RPosX, RPosY);
            vScreenPos = new Vector();
            bAsInput = AsInput;
        }
        public void Render(ref Graphics g, float Scale)
        {
            if(bState)
                g.FillEllipse(Brushes.Green, vScreenPos.x - nSize * Scale * 0.5f, 
                                             vScreenPos.y - nSize * Scale * 0.5f, nSize * Scale, nSize * Scale);
            else
                g.FillEllipse(Brushes.Red, vScreenPos.x - nSize * Scale * 0.5f, 
                                           vScreenPos.y - nSize * Scale * 0.5f, nSize * Scale, nSize * Scale);
        }
    }
    class NOT : LogicGate
    {
        Connection cInput;
        Connection cOutput;
        public NOT(float WPosX, float WPosY) : base(GateType.NOT, WPosX, WPosY)
        {
            cInput = new Connection(-nWidth * 0.6f, 0f, true);
            cOutput = new Connection(nWidth * 0.6f, 0f, false);
            cConnections = new Connection[] { cInput, cOutput };
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            bState = cInput.bState ? false : true;
            cOutput.bState = bState;
            cInput.vScreenPos = WorldToScreen(vWorldPos + cInput.vRelativePos, offset, Scale);
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cInput.Render(ref g, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class AND : LogicGate
    {
        Connection cInputOne;
        Connection cInputTwo;
        Connection cOutput;
        public AND(float WPosX, float WPosY) : base(GateType.AND, WPosX, WPosY) 
        {
            cInputOne = new Connection(-nWidth * 0.6f, -nHeight * 0.25f, true);
            cInputTwo = new Connection(-nWidth * 0.6f, nHeight * 0.25f, true);
            cOutput = new Connection(nWidth * 0.6f, 0f, false);
            cConnections = new Connection[] { cInputOne, cInputTwo, cOutput };
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            bState = (cInputOne.bState && cInputTwo.bState) ? true : false;
            cOutput.bState = bState;
            cInputOne.vScreenPos = WorldToScreen(vWorldPos + cInputOne.vRelativePos, offset, Scale);
            cInputTwo.vScreenPos = WorldToScreen(vWorldPos + cInputTwo.vRelativePos, offset, Scale);
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cInputOne.Render(ref g, Scale);
            cInputTwo.Render(ref g, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class OR : LogicGate
    {
        Connection cInputOne;
        Connection cInputTwo;
        Connection cOutput;
        public OR(float WPosX, float WPosY) : base(GateType.OR, WPosX, WPosY) 
        {
            cInputOne = new Connection(-nWidth * 0.6f, -nHeight * 0.25f, true);
            cInputTwo = new Connection(-nWidth * 0.6f, nHeight * 0.25f, true);
            cOutput = new Connection(nWidth * 0.6f, 0f, false);
            cConnections = new Connection[] { cInputOne, cInputTwo, cOutput };
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            bState = (!cInputOne.bState && !cInputTwo.bState) ? false : true;
            cOutput.bState = bState;
            cInputOne.vScreenPos = WorldToScreen(vWorldPos + cInputOne.vRelativePos, offset, Scale);
            cInputTwo.vScreenPos = WorldToScreen(vWorldPos + cInputTwo.vRelativePos, offset, Scale);
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cInputOne.Render(ref g, Scale);
            cInputTwo.Render(ref g, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class NOR : LogicGate
    {
        Connection cInputOne;
        Connection cInputTwo;
        Connection cOutput;
        public NOR(float WPosX, float WPosY) : base(GateType.NOR, WPosX, WPosY)
        {
            cInputOne = new Connection(-nWidth * 0.6f, -nHeight * 0.25f, true);
            cInputTwo = new Connection(-nWidth * 0.6f, nHeight * 0.25f, true);
            cOutput = new Connection(nWidth * 0.6f, 0f, false);
            cConnections = new Connection[] { cInputOne, cInputTwo, cOutput };
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            bState = (!cInputOne.bState && !cInputTwo.bState) ? true : false;
            cOutput.bState = bState;
            cInputOne.vScreenPos = WorldToScreen(vWorldPos + cInputOne.vRelativePos, offset, Scale);
            cInputTwo.vScreenPos = WorldToScreen(vWorldPos + cInputTwo.vRelativePos, offset, Scale);
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cInputOne.Render(ref g, Scale);
            cInputTwo.Render(ref g, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class NAND : LogicGate
    {
        Connection cInputOne;
        Connection cInputTwo;
        Connection cOutput;
        public NAND(float WPosX, float WPosY) : base(GateType.NAND, WPosX, WPosY)
        {
            cInputOne = new Connection(-nWidth * 0.6f, -nHeight * 0.25f, true);
            cInputTwo = new Connection(-nWidth * 0.6f, nHeight * 0.25f, true);
            cOutput = new Connection(nWidth * 0.6f, 0f, false);
            cConnections = new Connection[] { cInputOne, cInputTwo, cOutput };
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            bState = (cInputOne.bState && cInputTwo.bState) ? false : true;
            cOutput.bState = bState;
            cInputOne.vScreenPos = WorldToScreen(vWorldPos + cInputOne.vRelativePos, offset, Scale);
            cInputTwo.vScreenPos = WorldToScreen(vWorldPos + cInputTwo.vRelativePos, offset, Scale);
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cInputOne.Render(ref g, Scale);
            cInputTwo.Render(ref g, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class XNOR : LogicGate
    {
        Connection cInputOne;
        Connection cInputTwo;
        Connection cOutput;
        public XNOR(float WPosX, float WPosY) : base(GateType.XNOR, WPosX, WPosY)
        {
            cInputOne = new Connection(-nWidth * 0.6f, -nHeight * 0.25f, true);
            cInputTwo = new Connection(-nWidth * 0.6f, nHeight * 0.25f, true);
            cOutput = new Connection(nWidth * 0.6f, 0f, false);
            cConnections = new Connection[] { cInputOne, cInputTwo, cOutput };
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            bState = ((!cInputOne.bState && !cInputTwo.bState) || (cInputOne.bState && cInputTwo.bState)) ? true : false;
            cOutput.bState = bState;
            cInputOne.vScreenPos = WorldToScreen(vWorldPos + cInputOne.vRelativePos, offset, Scale);
            cInputTwo.vScreenPos = WorldToScreen(vWorldPos + cInputTwo.vRelativePos, offset, Scale);
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cInputOne.Render(ref g, Scale);
            cInputTwo.Render(ref g, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class XOR : LogicGate
    {
        Connection cInputOne;
        Connection cInputTwo;
        Connection cOutput;
        public XOR(float WPosX, float WPosY) : base(GateType.XOR, WPosX, WPosY)
        {
            cInputOne = new Connection(-nWidth * 0.6f, -nHeight * 0.25f, true);
            cInputTwo = new Connection(-nWidth * 0.6f, nHeight * 0.25f, true);
            cOutput = new Connection(nWidth * 0.6f, 0f, false);
            cConnections = new Connection[] { cInputOne, cInputTwo, cOutput };
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            bState = ((!cInputOne.bState && !cInputTwo.bState) || (cInputOne.bState && cInputTwo.bState)) ? false : true;
            cOutput.bState = bState;
            cInputOne.vScreenPos = WorldToScreen(vWorldPos + cInputOne.vRelativePos, offset, Scale);
            cInputTwo.vScreenPos = WorldToScreen(vWorldPos + cInputTwo.vRelativePos, offset, Scale);
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cInputOne.Render(ref g, Scale);
            cInputTwo.Render(ref g, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class IN : LogicGate
    {
        Connection cOutput;
        public IN(float WPosX, float WPosY) : base(GateType.IN, WPosX, WPosY)
        {
            cOutput = new Connection(0f, 0f, false);
            cConnections = new Connection[] { cOutput };
            cOutput.nSize = nWidth;
            bState = true;
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            cOutput.bState = bState;
            cOutput.vScreenPos = WorldToScreen(vWorldPos + cOutput.vRelativePos, offset, Scale);
            cOutput.Render(ref g, Scale);
        }
    }
    class OUT : LogicGate
    {
        Connection cInput;
        public OUT(float WPosX, float WPosY) : base(GateType.OUT, WPosX, WPosY)
        {
            cInput = new Connection(0f, 0f, true);
            cConnections = new Connection[] { cInput };
            cInput.nSize = nWidth;
        }
        public override void Render(ref Graphics g, Vector offset, float Scale)
        {
            base.Render(ref g, offset, Scale);
            cInput.vScreenPos = WorldToScreen(vWorldPos + cInput.vRelativePos, offset, Scale);
            cInput.Render(ref g, Scale);
        }
    }
}
