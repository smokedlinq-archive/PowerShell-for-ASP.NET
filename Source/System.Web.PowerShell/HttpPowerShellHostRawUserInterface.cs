using System.Management.Automation.Host;

namespace System.Web.PowerShell
{
    internal sealed class HttpPowerShellHostRawUserInterface : PSHostRawUserInterface
    {
        static readonly Size DefaultSize = new Size();
        static readonly Coordinates DefaultCoordinates = new Coordinates();

        public override ConsoleColor BackgroundColor
        {
            get
            {
                return ConsoleColor.Black;
            }
            set
            {
            }
        }

        public override Size BufferSize
        {
            get
            {
                return DefaultSize;
            }
            set
            {
            }
        }

        public override Coordinates CursorPosition
        {
            get
            {
                return DefaultCoordinates;
            }
            set
            {
            }
        }

        public override int CursorSize
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public override void FlushInputBuffer()
        {
        }

        public override ConsoleColor ForegroundColor
        {
            get
            {
                return ConsoleColor.White;
            }
            set
            {
            }
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            return null;
        }

        public override bool KeyAvailable
        {
            get { return false; }
        }

        public override Size MaxPhysicalWindowSize
        {
            get { return DefaultSize; }
        }

        public override Size MaxWindowSize
        {
            get { return DefaultSize; }
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotSupportedException();
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException();
        }

        public override Coordinates WindowPosition
        {
            get
            {
                return DefaultCoordinates;
            }
            set
            {
            }
        }

        public override Size WindowSize
        {
            get
            {
                return DefaultSize;
            }
            set
            {
            }
        }

        public override string WindowTitle
        {
            get
            {
                return "System.Web.PowerShell";
            }
            set
            {
            }
        }
    }
}
