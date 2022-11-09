<h1 align="center">
  <img width="256" src="https://github.com/rozbo/VisualStudio-TrackCommandId/blob/main/TrackCommandId/logo.png">
  <br>Command Tracker</br>
<h1>

## the story

我是一个vim的忠实粉丝，即使在VisualStudio里也不例外，事实上我一直很排斥VisualStudio，其中一个重要的原因就是它没有`ideavim`，但近日来经济拮据，无力续费 `rider`，没办法重试VisualStudio和`VsVim`，在折腾配置的时候，需要用可编程的方式调用vs的一些命令，但无奈，vs有超过1000个命令，这实在是一项重大的工程。    
[https://github.com/JetBrains/ideavim#finding-action-ids](https://github.com/JetBrains/ideavim#finding-action-ids)在ideavim里就做的很好，它内置了一个功能--`track action ids`，我想在vs里实现同样的功能，我不知道是否自带了这样的功能，反正我是没找到。但我找到了这个[ https://learn.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-in-visual-studio?view=vs-2022#bkmk_edit-popular-shortcuts](https://learn.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-in-visual-studio?view=vs-2022#bkmk_edit-popular-shortcuts)


后来我突然想到会不会有个一个插件来实现类似的事情呢，经过一番查找，我找到了这个[https://marketplace.visualstudio.com/items?itemName=MadsKristensen.LearntheShortcut](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.LearntheShortcut) 但遗憾的是，它不支持我所使用的vs2022，经过查看它的代码[https://github.com/madskristensen/ShowTheShortcut](https://github.com/madskristensen/ShowTheShortcut)，已经三年没有更新了。。


经过一番思想挣扎，理智战胜了懒惰，我决定重写它。
