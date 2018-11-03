﻿EnsureDataLoaded();

static Random rng = new Random();

static void Shuffle<T>(this IList<T> list)
{
    int n = list.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);

        T value = list[k];
        list[k] = list[n];
        list[n] = value;
    }
}

static void ShuffleOnlySelected<T>(this IList<T> list, IList<int> selected, Action<int, int> swapFunc)
{
    int n = selected.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);

        swapFunc(selected[n], selected[k]);

        int idx = selected[k];
        selected[k] = selected[n];
        selected[n] = idx;
    }
}

static void ShuffleOnlySelected<T>(this IList<T> list, IList<int> selected)
{
    list.ShuffleOnlySelected(selected, (n, k) => {
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
    });
}

static void SelectSome(this IList<int> list, float amountToKeep)
{
    int toRemove = (int)(list.Count * (1 - amountToKeep));
    for (int i = 0; i < toRemove; i++)
        list.RemoveAt(rng.Next(list.Count));
}

List<int> tiny = new List<int>();
List<int> small = new List<int>();
List<int> characterlike = new List<int>();
List<int> big = new List<int>();
for (int i = 0; i < Data.Sprites.Count; i++)
{
    var sprite = Data.Sprites[i];
    if (sprite.Name.Content.StartsWith("bg_"))
        continue;
    if (sprite.Width < 50 && sprite.Height < 50)
        tiny.Add(i);
    else if (sprite.Width < 50 && sprite.Height < 100)
        characterlike.Add(i);
    else if (sprite.Width < 100 && sprite.Height < 100)
        small.Add(i);
    else if (sprite.Width < 200 && sprite.Height < 200)
        big.Add(i);
}
tiny.SelectSome(0.4f);
small.SelectSome(0.4f);
characterlike.SelectSome(0.4f);
big.SelectSome(0.4f);
Data.Sprites.ShuffleOnlySelected(tiny);
Data.Sprites.ShuffleOnlySelected(small);
Data.Sprites.ShuffleOnlySelected(characterlike);
Data.Sprites.ShuffleOnlySelected(big);

Data.Sounds.Shuffle();

List<int> en_fonts = new List<int>();
List<int> ja_fonts = new List<int>();
for (int i = 0; i < Data.Fonts.Count; i++)
{
    if (Data.Fonts[i].Name.Content.StartsWith("fnt_ja_"))
        ja_fonts.Add(i);
    else
        en_fonts.Add(i);
}
Data.Fonts.ShuffleOnlySelected(en_fonts);
Data.Fonts.ShuffleOnlySelected(ja_fonts);

Dictionary<string, string> translations = new Dictionary<string, string>();
foreach (string line in File.ReadAllLines(Path.Combine(Path.GetDirectoryName(FilePath), "lang/lang_en.json")))
{
    // Yeah. No JSON support in scripts. Deal with it.
    string[] a = line.Split(new char[] { ':' }, 2);
    if (a.Length != 2)
        continue;
    a[0] = a[0].Trim();
    a[0] = a[0].Substring(1, a[0].Length - 2);
    a[1] = a[1].Trim();
    a[1] = a[1].Substring(1, a[1].Length - 3);
    if (a[0] == "date")
        continue;
    if (a[1] == "||") // This breaks the auto-line-break badly, why is this string even localized
        continue;
    translations.Add(a[0], a[1]);
}
// Splitting the strings into groups like this is necessary to prevent crashes and the game freezing because of waiting on input it can't get
List<int> choicer_old_lines = new List<int>();
List<int> choicer_neo_2_lines = new List<int>();
List<int> choicer_neo_3_lines = new List<int>();
List<int> choicer_neo_4_lines = new List<int>();
List<int> final_lines = new List<int>();
List<int> continue_lines = new List<int>();
List<int> waiting_lines = new List<int>();
List<int> waiting_final_lines = new List<int>();
List<int> waiting_continue_lines = new List<int>();
List<int> dash_whatever_that_is = new List<int>();
List<int> other_lines = new List<int>();
for (int i = 0; i < Data.Strings.Count; i++)
{
    var id = Data.Strings[i].Content;
    if (translations.ContainsKey(id))
    {
        var str = translations[id];
        if (str.Contains("\\\\C1"))
            choicer_old_lines.Add(i);
        else if (str.Contains("\\\\C2"))
            choicer_neo_2_lines.Add(i);
        else if (str.Contains("\\\\C3"))
            choicer_neo_3_lines.Add(i);
        else if (str.Contains("\\\\C4"))
            choicer_neo_4_lines.Add(i);
        else if (str.EndsWith("/%%"))
            waiting_final_lines.Add(i);
        else if (str.EndsWith("/%"))
            waiting_continue_lines.Add(i);
        else if (str.EndsWith("/"))
            waiting_lines.Add(i);
        else if (str.EndsWith("%%"))
            final_lines.Add(i);
        else if (str.EndsWith("%"))
            continue_lines.Add(i);
        else if (str.EndsWith("-"))
            dash_whatever_that_is.Add(i);
        else
            other_lines.Add(i);
    }
}
// We have to swap the contents because UndertaleModTool is too smart :P
void StringSwap(int n, int k)
{
    string value = Data.Strings[k].Content;
    Data.Strings[k].Content = Data.Strings[n].Content;
    Data.Strings[n].Content = value;
}
Data.Strings.ShuffleOnlySelected(choicer_old_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(choicer_neo_2_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(choicer_neo_3_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(choicer_neo_4_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(waiting_final_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(waiting_continue_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(waiting_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(final_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(continue_lines, StringSwap);
Data.Strings.ShuffleOnlySelected(dash_whatever_that_is, StringSwap);
Data.Strings.ShuffleOnlySelected(other_lines, StringSwap);

ScriptMessage("* GASTER NOISES *\n\nIT'S DONE");