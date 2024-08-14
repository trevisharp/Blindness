//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections.Generic;
using Blindness;
using Blindness.Bind;
using Blindness.Core;

[Concrete]
public partial class LoginScreenConcrete : Node, LoginScreen
{
	public LoginScreenConcrete() { }
	public void Deps(
		Panel Panel,
		TextBox Login,
		TextBox Password,
		TextBox Repeat
	)
	{
		this.Panel = Panel;
		this.Login = Login;
		this.Password = Password;
		this.Repeat = Repeat;
	}
	public override void Load()
	    => ((LoginScreen)this).OnLoad();
	public override void Run()
	    => ((LoginScreen)this).OnRun();
	[Binding]
	public Panel Panel
	{
		get => Binding.Get(this).Open<Panel>(nameof(Panel));
		set => Binding.Get(this).Place(nameof(Panel), value);
	}
	[Binding]
	public TextBox Login
	{
		get => Binding.Get(this).Open<TextBox>(nameof(Login));
		set => Binding.Get(this).Place(nameof(Login), value);
	}
	[Binding]
	public TextBox Password
	{
		get => Binding.Get(this).Open<TextBox>(nameof(Password));
		set => Binding.Get(this).Place(nameof(Password), value);
	}
	[Binding]
	public TextBox Repeat
	{
		get => Binding.Get(this).Open<TextBox>(nameof(Repeat));
		set => Binding.Get(this).Place(nameof(Repeat), value);
	}
	[Binding]
	public String login
	{
		get => Binding.Get(this).Open<String>(nameof(login));
		set => Binding.Get(this).Place(nameof(login), value);
	}
	[Binding]
	public String password
	{
		get => Binding.Get(this).Open<String>(nameof(password));
		set => Binding.Get(this).Place(nameof(password), value);
	}
	[Binding]
	public String repeat
	{
		get => Binding.Get(this).Open<String>(nameof(repeat));
		set => Binding.Get(this).Place(nameof(repeat), value);
	}
	[Binding]
	public Boolean registerPage
	{
		get => Binding.Get(this).Open<Boolean>(nameof(registerPage));
		set => Binding.Get(this).Place(nameof(registerPage), value);
	}
	[Binding]
	public Int32 selectedField
	{
		get => Binding.Get(this).Open<Int32>(nameof(selectedField));
		set => Binding.Get(this).Place(nameof(selectedField), value);
	}
	[Binding]
	public List<INode> children
	{
		get => Binding.Get(this).Open<List<INode>>(nameof(children));
		set => Binding.Get(this).Place(nameof(children), value);
	}

}