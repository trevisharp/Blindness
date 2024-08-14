﻿using System;
using System.Text;
using System.Collections.Generic;

using Blindness;
using Blindness.Core;

Verbose.VerboseLevel = int.MaxValue;
App.Start<LoginScreen>();

public interface LoginScreen : INode
{
    Panel Panel { get; set; }
    TextBox Login { get; set; }
    TextBox Password { get; set; }
    TextBox Repeat { get; set; }
    string login { get; set; }
    string password { get; set; }
    string repeat { get; set; }
    bool registerPage { get; set; }
    int selectedField { get; set; }
    List<INode> children { get; set; }

    void Deps(
        Panel Panel, 
        TextBox Login, 
        TextBox Password,
        TextBox Repeat
    );

    void OnLoad()
    {
        Panel.Title = "Register Page";
        Panel.Width = 60;

        Login.Title = "login";
        Login.Size = 40;

        Password.Title = "password";
        Password.Text = "";
        Password.Size = 40;

        Repeat.Title = "repeat password";
        Repeat.Size = 40;

        Bind(() => login == Login.Text);
        Bind(() => password == Password.Text);
        Bind(() => repeat == Repeat.Text);
        Bind(() => children == Panel.Children);
        Bind(() => Login.Selected == (selectedField == 0));
        Bind(() => Password.Selected == (selectedField == 1));
        Bind(() => Repeat.Selected == (selectedField == 2));

        registerPage = true;

        When(
            () => registerPage,
            () => children = [ Login, Password, Repeat ]
        );

        When(
            () => !registerPage,
            () => children = [ Login, Password ]
        );

        On(
            () => 
                !registerPage ||
                password.Length > 5  &&
                password.Length < 50 &&
                password == repeat,
            r =>
                Password.Title = r ? 
                "password" :
                "password (has errors)"
        );
    }

    void OnRun()
    {
        Console.Clear();
        Panel.Run();

        var newChar = Console.ReadKey(true);
        if (newChar.Key == ConsoleKey.Tab)
        {
            selectedField = (selectedField, registerPage) switch
            {
                (2, true) => 0,
                (1, false) => 0,
                (var n, _) => n + 1
            };
            return;
        }

        if (newChar.Key == ConsoleKey.Backspace)
        {
            switch (selectedField)
            {
                case 0:
                    if (login.Length == 0)
                        break;
                    login = login[..^1];
                    break;

                case 1:
                    if (password.Length == 0)
                        break;
                    password = password[..^1];
                    break;

                case 2:
                    if (repeat.Length == 0)
                        break;
                    repeat = repeat[..^1];
                    break;
            }
            return;
        }

        if (newChar.Key == ConsoleKey.Enter)
        {
            Panel.Title = "Login Page";
            registerPage = false;
            selectedField = 0;
            return;
        }

        switch (selectedField)
        {
            case 0:
                login += newChar.KeyChar;
                break;

            case 1:
                password += newChar.KeyChar;
                break;

            case 2:
                repeat += newChar.KeyChar;
                break;
        }
    }
}

public interface Panel : INode
{
    string Title { get; set; }
    int Width { get; set; }
    List<INode> Children { get; set; }

    void OnRun()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("─ ");
        sb.Append(Title);
        sb.Append(" ");
        sb.Append('─', Width - Title.Length - 2);
        Console.WriteLine(sb);

        foreach (var child in Children)
            child.Run();

        sb.Clear();
        sb.Append('─', Width);
        Console.WriteLine(sb);
    }
}

public interface TextBox : INode
{
    string Title { get; set; }
    string Text { get; set; }
    int Size { get; set; }
    bool Selected { get; set; }

    void OnRun()
    {
        StringBuilder sb = new StringBuilder();
        int size = 8 * Size / 10;
        Text ??= "";
        var text = Text.Length < size ? Text : 
            Text.Substring(
            Text.Length - size, size
        );

        if (Selected)
        {
            sb.Append("╔");
            sb.Append(Title);
            sb.Append('═', Size + 2 - Title.Length);
            sb.AppendLine("╗");

            sb.Append("║ ");
            sb.Append(text);
            sb.Append(' ', Size + 1 - text.Length);
            sb.AppendLine("║");

            sb.Append("╚");
            sb.Append('═', Size + 2);
            sb.AppendLine("╝");
        }
        else
        {
            sb.Append("┌");
            sb.Append(Title);
            sb.Append('─', Size + 2 - Title.Length);
            sb.AppendLine("┐");

            sb.Append("│ ");
            sb.Append(text);
            sb.Append(' ', Size + 1 - text.Length);
            sb.AppendLine("│");

            sb.Append("└");
            sb.Append('─', Size + 2);
            sb.AppendLine("┘");
        }

        Console.WriteLine(sb);
    }
}