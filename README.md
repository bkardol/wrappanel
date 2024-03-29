Welcome to the WrapPanel repository!

[![NuGet Downloads](https://img.shields.io/nuget/dt/WrapPanel.UWP.svg)](https://www.nuget.org/packages/WrapPanel.UWP/)

# Info
As the VariableSizedWrapGrid and ItemsWrapGrid both take either user defined sizes or take the size of the first child it isn't easy to display a collection of items with various sizes.
The WrapPanel control is created to support these circumstances.

The repository contains two directories:
* WrapPanel, containing the source of the WrapPanel itself;
* WrapPanel.Example, containing an example application demonstrating the use of the WrapPanel.


# Quick-Start
In your xaml page add the following namespace: 
```xml
xmlns:wp="using:WrapPanel"
```

After, you can start using the WrapPanel:
```xml
<wp:WrapPanel ItemsSource="{x:Bind Blocks, Mode=OneWay}">
  <wp:WrapPanel.ItemTemplate>
    <DataTemplate x:DataType="local:Block">
      <Border Background="Black" Width="100" Height="100" Margin="10" />
    </DataTemplate>
  </wp:WrapPanel.ItemTemplate>
</wp:WrapPanel>
```

Below, a screenshot of the WrapPanel in action where all children are sorted by size. 

![wrappanel example](_images/WrapPanel.Example.png)
