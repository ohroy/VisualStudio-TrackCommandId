<h1 align="center">
  <img width="256" src="https://github.com/rozbo/VisualStudio-TrackCommandId/blob/main/TrackCommandId/logo.png">
  <br>Command Tracker</br>
<h1>

## the story

我是一个vim的忠实粉丝，即使在VisualStudio里也不例外，事实上我一直很排斥VisualStudio，其中一个重要的原因就是它没有`ideavim`，但近日来经济拮据，无力续费 `rider`，没办法重试VisualStudio和`VsVim`，在折腾配置的时候，需要用可编程的方式调用vs的一些命令，但无奈，vs有超过1000个命令，这实在是一项重大的工程。    
[https://github.com/JetBrains/ideavim#finding-action-ids](https://github.com/JetBrains/ideavim#finding-action-ids)在ideavim里就做的很好，它内置了一个功能--`track action ids`，我想在vs里实现同样的功能，我不知道是否自带了这样的功能，反正我是没找到。但我找到了这个[ https://learn.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-in-visual-studio?view=vs-2022#bkmk_edit-popular-shortcuts](https://learn.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-in-visual-studio?view=vs-2022#bkmk_edit-popular-shortcuts)


后来我突然想到会不会有个一个插件来实现类似的事情呢，经过一番查找，我找到了这个[https://marketplace.visualstudio.com/items?itemName=MadsKristensen.LearntheShortcut](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.LearntheShortcut) 但遗憾的是，它不支持我所使用的vs2022，经过查看它的代码[https://github.com/madskristensen/ShowTheShortcut](https://github.com/madskristensen/ShowTheShortcut)，已经三年没有更新了。。


经过一番思想挣扎，理智战胜了懒惰，我决定重写它。


## the usage


### how to install
你可以通过vs内置的扩展管理里或者官方商店[https://marketplace.visualstudio.com/items?itemName=rozbo.command-tracker](https://marketplace.visualstudio.com/items?itemName=rozbo.command-tracker)下载，也可以在本仓库的[release](https://github.com/rozbo/VisualStudio-TrackCommandId/releases)页面下载。


### how to use it

你可以在设置里，启用本插件，根据你的配置，可以选择输出到状态栏、输出窗口、或者直接输出到剪切板，此后，每当你鼠标点击一个菜单，在以上的输出位置即可输出此命令的`command name`和`short key`(如果它绑定了快捷键的话)。

