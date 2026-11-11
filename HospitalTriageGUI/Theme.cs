using HospitalTriage;

namespace HospitalTriageGUI
{
    /// <summary>
    /// Shared colours, fonts and small factory methods so every screen looks consistent.
    /// </summary>
    internal static class Theme
    {
        public static readonly Color Navy = Color.FromArgb(25, 43, 70);
        public static readonly Color NavyHover = Color.FromArgb(45, 67, 100);
        public static readonly Color NavyActive = Color.FromArgb(48, 112, 190);
        public static readonly Color Blue = Color.FromArgb(48, 112, 190);
        public static readonly Color Background = Color.FromArgb(245, 247, 251);
        public static readonly Color Border = Color.FromArgb(222, 227, 235);
        public static readonly Color Muted = Color.FromArgb(110, 120, 135);

        public static readonly Color Red = Color.FromArgb(210, 65, 65);
        public static readonly Color Yellow = Color.FromArgb(225, 166, 40);
        public static readonly Color Green = Color.FromArgb(55, 155, 90);

        public static readonly Color RedTint = Color.FromArgb(253, 234, 234);
        public static readonly Color YellowTint = Color.FromArgb(255, 246, 222);
        public static readonly Color GreenTint = Color.FromArgb(230, 245, 235);

        public static Font F(float size, FontStyle style = FontStyle.Regular) => new("Segoe UI", size, style);

        public static Color PriorityColor(PriorityLevel p) => p switch
        {
            PriorityLevel.Red => Red,
            PriorityLevel.Yellow => Yellow,
            _ => Green
        };

        public static Color PriorityTint(PriorityLevel p) => p switch
        {
            PriorityLevel.Red => RedTint,
            PriorityLevel.Yellow => YellowTint,
            _ => GreenTint
        };

        public static Color PrioritySelection(PriorityLevel p) => p switch
        {
            PriorityLevel.Red => Color.FromArgb(240, 190, 190),
            PriorityLevel.Yellow => Color.FromArgb(245, 222, 160),
            _ => Color.FromArgb(180, 225, 195)
        };

        public static string PriorityDescription(PriorityLevel p) => p switch
        {
            PriorityLevel.Red => "Immediate attention",
            PriorityLevel.Yellow => "Urgent attention",
            _ => "Non-critical"
        };

        public static string FormatWait(TimeSpan t)
        {
            if (t < TimeSpan.Zero) t = TimeSpan.Zero;
            if (t.TotalHours >= 1) return $"{(int)t.TotalHours}h {t.Minutes:00}m";
            return $"{t.Minutes}m {t.Seconds:00}s";
        }

        public static Label MakeLabel(string text, float size, FontStyle style, Color color, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = F(size, style),
                ForeColor = color,
                Location = new Point(x, y),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
        }

        public static Button MakeButton(string text, Color back, Color fore, int width, int height = 42)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(width, height),
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Font = F(10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = back == Color.White ? 1 : 0;
            button.FlatAppearance.BorderColor = Border;
            return button;
        }

        public static Button PrimaryButton(string text, int width, int height = 42) =>
            MakeButton(text, Blue, Color.White, width, height);

        public static Button SecondaryButton(string text, int width, int height = 42)
        {
            var b = MakeButton(text, Color.White, Navy, width, height);
            b.Font = F(10);
            return b;
        }

        /// <summary>A white panel with a thin border, used as a "card".</summary>
        public static Panel Card(int x, int y, int width, int height)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White
            };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Border);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            return card;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Navy;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Navy;
            grid.ColumnHeadersDefaultCellStyle.Font = F(9.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle.Font = F(9.5f);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 230, 248);
            grid.DefaultCellStyle.SelectionForeColor = Navy;
            grid.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
            grid.RowTemplate.Height = 34;
        }

        public static void AddColumns(DataGridView grid, params (string Header, float Weight)[] columns)
        {
            foreach (var (header, weight) in columns)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = header,
                    Name = header,
                    FillWeight = weight,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
            }
        }

        /// <summary>Applies priority colours to a grid row.</summary>
        public static void ColourRow(DataGridViewRow row, PriorityLevel priority, string priorityColumn)
        {
            row.DefaultCellStyle.BackColor = PriorityTint(priority);
            row.DefaultCellStyle.SelectionBackColor = PrioritySelection(priority);
            var cell = row.Cells[priorityColumn];
            cell.Style.ForeColor = PriorityColor(priority);
            cell.Style.SelectionForeColor = PriorityColor(priority);
            cell.Style.Font = F(9.5f, FontStyle.Bold);
        }
    }
}
