# Andrew.GameOfLifeDemo

這是個練習 OOP 觀念的題目。取材自 computer sciense 的經典練習題: [康威生命遊戲](https://zh.wikipedia.org/wiki/%E5%BA%B7%E5%A8%81%E7%94%9F%E5%91%BD%E6%B8%B8%E6%88%8F)。

計算的規則請參考上面的網頁介紹，本練習分成四個里程碑，分別定義如下:



## Milestone 1, 基本練習

沒有特別的要求，目的是讓你暖暖身，熟悉一下題目的規則用的。
請按照 branch:main 提供的 GameHost1/Program.cs ，將你的 code 填入 TimePassRule() 內:

```csharp

public static bool TimePassRule(bool[,] area)
{
    // TODO: fill your code here
    return area[1, 1];
}

```

編譯成功後，執行應該能夠看到細胞演進過程。請確認能通過所有的單元測試為過關。


## Milestone 2, OOP 版本

請用 OOP 的觀念來重新開發這個程式。你可以選擇直接修改 ```static bool[,] GetNextGenMatrix(bool[,] matrix_current)``` 的內容，在裡面使用你定義的各種 class / object 來完成同樣的任務。因為不再必要填入 TimePassRule(), 因此單元測試只需要通過 ```GetNextGenMatrixTest.cs``` 即可。同樣的，編譯成功執行後能看到細胞演進過程，並且能通過單元測試為過關。

以上是最低的要求 (結果正確，必須通過測試)。改用 OOP 版本的目的是期待用正確的 class 來對應實際的世界，每段 code 都能夠歸屬在合適的 class / instance 身上。正確的對應就能適應現實世界的需求變化，因此是否正確的抽象化 (能否在 ADT 的規範下安全的擴充)，是否正確的封裝 (避免不必要或是錯誤的存取) 都是 OOP 化考量的重點。

以下是 OOP 抽象化定義的建議，若你不確定該怎麼作，可以參考看看:

1. 你要如何描述 "世界" ? 他提供那些操作? 可操作的對象有哪些?
1. 你要如何描述 "細胞" ? 他提供那些操作? 可操作的對象有哪些?
1. 如何 init 這個 "世界" ? 如何從無到有的把每個細胞 "生" 出來，並且擺到合適的地方?
1. 細胞如何 "看" 到周圍有多少其他細胞? 如果細胞的 code 沒有寫好，能不能看到超過它周圍的其他細胞? (例如: 能否看到 5 x 5 的所有細胞狀態? )
1. 承 (4), 如果不行，那你該如何把整個世界的狀態顯示出來? (4) 跟 (5) 的需求是衝突的 ( 只能看到 3x3 vs 看到整個世界地圖 ) 該如何解決?
1. 如果需要，可以引入 "神" 的角色，來收斂管理這些需要特權的行為

這些關係想清楚後，你的 code 應該會出現 "世界"、"細胞"、"神" 這幾個類別，並且確定 "細胞" 無法執行某些只有 "神" 才能執行的動作。

> 如果你是我公司的同仁，那麼通過條件多一個: 請說明你這樣設計 class / object 背後的想法。可以透過 class diagram 或是 UML use case diagram 來說明。

## Milestone 3, OOP + 非固定周期演進

原始康威生命遊戲的規則，是以 "回合" 為基礎。每個細胞在下一個周期時，都會按照同樣的規則，在各自的狀態下演進到下個世代。這邊修改一下題目的規則，如果每個細胞演進到下個世代的時間為變數的話，那這個模擬生命的程式該如何開發?

這邊補充幾個規則，讓題目明確一點:

1. Init() 時，除了指派每格的初始狀態為 true | false 之外，也會指派每次演進的時間週期 (單位為 msec, 範圍為 10 ~ 100 msec)。
1. 外界環境 (matrix) 會以固定的週期 (預設: 10 msec ) 刷新。在每個周期內的細胞，只能感知到上個週期身邊細胞的狀態。  
例如，在 300ms ~ 310ms 時段內，A 細胞看到她周圍細胞狀態，會是 290ms ~ 300ms 期間內的結果。A 細胞在他自身週期到的時候，改變的狀態，其他細胞要在下個週期 ( 310ms ~ 320ms ) 才能感知的到。
1. 如果細胞跟環境的刷新時間剛好都重疊再一起，則變化順序以細胞的演進為優先，這瞬間演進的結果會出現在這次的環境刷新內。
1. 如果外界環境週期固定為 10ms, 所有細胞從 5ms 開始，周期也固定為 10ms, 那麼所有的表現都應該跟 milestone 2 一樣才對。

我用個案例來實際說明一下上面的規則。舉例來說，我有一個 6 x 6 的空間，初始值如下圖 (我直接取用 wiki 說明的 `蟾蜍` 當案例):

![](https://upload.wikimedia.org/wikipedia/commons/1/12/Game_of_life_toad.gif)


化成文字，初始狀態是這樣:

x x x x x x  
x x x x x x  
x x o o o x  
x o o o x x  
x x x x x x  
x x x x x x  

按照原始的規則，下一個周期應該會變成:

x x x x x x  
x x x o x x  
x o x x o x  
x o x x o x  
x x o x x x  
x x x x x x  

我就用這個 case 說明一下加入時間維度的差別。假設時間我都用 msec 為單位，環境每 10 ms 刷新一次，而每個細胞刷新時間為 5 ms, 那麼:

初始狀態如左圖，右圖是預計下次刷新的狀態。每個細胞都是用左圖的狀態為依據來判定周圍細胞的狀態。而在下次刷新的狀態真正到達刷新時間前，都可能被任何一個細胞演進的結果更動。


```
x x x x x x         x x x x x x   
x x x x x x         x x x o x x  
x x o o o x         x o x x o x  
x o o o x x         x o x x o x  
x x x x x x         x x o x x x  
x x x x x x         x x x x x x  
  
snapshot(0ms)       current(5ms)  
```

不過，5ms 後環境還沒刷新啊，因此細胞本身狀態可能已經改變了，但是他還感知不到周圍的細胞狀態改變了。因此我們再看看時間軸從 5ms 到 10ms 會發生啥事:

```
x x x x x x         x x x x x x         x x x x x x   
x x x x x x         x x x o x x         x x x o x x  
x x o o o x         x o x x o x         x o x x o x  
x o o o x x         x o x x o x         x o x x o x  
x x x x x x         x x o x x x         x x o x x x  
x x x x x x         x x x x x x         x x x x x x  
  
snapshot(0ms)       current(5ms)        current(10ms)  
```

結果剛剛好沒有改變。10ms 過了之後，環境也被刷新了，因此 snapshot(10ms) 的內容應該就會變成 current(10ms) 的狀態了。

```
x x x x x x         x x x x x x         x x x x x x       x x x x x x          
x x x x x x         x x x o x x         x x x o x x       x x x o x x         
x x o o o x         x o x x o x         x o x x o x       x o x x o x          
x o o o x x         x o x x o x         x o x x o x       x o x x o x         
x x x x x x         x x o x x x         x x o x x x       x x o x x x         
x x x x x x         x x x x x x         x x x x x x       x x x x x x         
  
snapshot(0ms)       current(5ms)        current(10ms)     snapshot(10ms)      
```

不曉得我這樣表達方式是否清楚明確? 總之規則我說了算 XDD, 不清楚的話可以留 issue 給我。這邊特別提醒一下，雖然我定義了 "時間" 的維度，但是這次練習沒有必要用真正的
時間函數來處理啊! 舉例來說，你模擬的時候不用真的在 code 內呼叫 `Task.Delay(10).Wait();` 來處理 `過了 10ms` 這件事。你只要有辦法計算出符合上述規則下，下個環境
刷新後的狀態 ( snapshot ) 就可以了。時間的最小單位是 1 ms, 因此你不用擔心有 0.3 ms 這種狀況。

當然你要用 Task.Delay(10) 也沒問題的，不過你也許會伴隨著要去面對 Task.Delay(10) 背後的誤差問題。非同步呼叫通常都是讓 thread 進入 wait 狀態，時間到了才會被喚醒。但是喚醒不保證絕對精準啊，要看 OS 當下的判定。你用了 Task.Delay(10) 若無法準確的處理這些誤差問題的話，你可能就會讓判定的順序錯亂了。舉例來說，前面規則 (3) 提到環境跟細胞刷新時間如果都一樣的話，必須以細胞演進的處理為優先...。

跳過時間函數，也許有些細節要自己處理 (比如你要另外表示 `目前時間` 的方式，用來替代 DateTime.Now 本來的職責)，但是相對的好處是，你可以加速模擬的時間，不再受到時間的限制... (有點像 SAO underworld 的 1000x 加速啊啊啊 XDD)。

過關的標準，必須通過 GameHost2 隨附的單元測試 (還沒寫好...)，同時請務必維持程式碼的精簡。後半段的過關標準我無法量化，同樣的，如果是我公司的同仁，採取當面說明設計想法為準。

## Milestone 4, 大亂鬥

如果你以上三個里程碑都達標的話，我會釋出我的 Milestone 4 題目 (有標準定義的 ICell interface 需要實作)。我將會在相同限制條件下，允許每個參賽者調整部分的參數，一同放到同一個培養皿裡面生活。看看多個世代下去，哪個物種能夠佔據最多的數量? 你無法預測你的競爭對手參數會怎麼調整，只能自己想辦法提高求生機率。

> 題目還沒想好怎麼開，敬請期待 XDD

經過幾番奮鬥，能在眾多競爭者的求生存的情況下，存活到最後一刻的人...
...
...
我沒有獎品可以提供，只能提供口頭獎勵 XDD，請各位加油 :D




----

# 後記

開這個練習題，目的是訓練 team member 物件導向的思考能力。OOP 很強調 interface, 而良好的 interface 是開出良好的 API 的基礎。看過太多用了先進開發工具與框架的工程師，卻設計出很糟糕的 API 有感。

這系列里程碑的設計，其實是有目的的，我背後的用意，依序如下:

1. 就是讓你熟悉經典題目的內容，暖身練習用。
1. 試著用物件的觀念來重新寫一次題目。同時也預先劇透了後面的題目，讓你知道將來有哪些擴充的需求，做出正確的抽象化設計。
1. 加入時間的維度。傳統的程式設計都以單線流程推演為主，加入了時間維度會打破這個思維。我觀察到很多團隊都擅長用排程批次處理問題，而不擅長用平行處理，或是事件觸發的方式來處理同樣的問題，就是不熟悉以時間維度為主的思考方式。這個設計就是希望大家練習這種思考模式，並且將它應用在程式碼的撰寫上。
1. 大亂鬥，單純有競賽感，努力了三個回合後，最後讓大家能在一個共通的 GameHost 上用 code 彼此較量一番。