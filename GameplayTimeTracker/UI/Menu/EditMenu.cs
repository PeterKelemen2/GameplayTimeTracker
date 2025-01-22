using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameplayTimeTracker.Menu;

public class EditMenu : EntryConfigMenu
{
    public EditMenu(Entry entry,
        double width = 350, double height = 550, bool performanceMode = true)
        : base(entry, width, height, performanceMode)
    {
        TitleTextBlock.FontWeight = FontWeights.Regular;
        TitleTextBlock.Text = "Editing ";
        TitleTextBlock.Inlines.Add(new Run { Text = entry.Name, FontWeight = FontWeights.Bold });
        // ConfirmButton.Click += (_, _) => { AddConfiguredEntry(entry, entryRepo, cardRepo, panel); };
    }
}