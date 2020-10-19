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

> 如果你是我公司的同仁，那麼通過條件多一個: 請說明你這樣設計 class / object 背後的想法。可以透過 class diagram 或是 UML use case diagram 來說明。

## Milestone 3, OOP + 非固定周期演進

原始康威生命遊戲的規則，是以 "回合" 為基礎。每個細胞在下一個周期時，都會按照同樣的規則，在各自的狀態下演進到下個世代。這邊修改一下題目的規則，如果每個細胞演進到下個世代的時間為變數的話，那這個模擬生命的程式該如何開發?

這邊補充幾個規則，讓題目明確一點:

1. Init() 時，除了指派每格的初始狀態為 true | false 之外，也會指派每次演進的時間週期 (單位為 msec, 範圍為 10 ~ 100 msec)。
1. 外界環境 (matrix) 會以固定的週期 (預設: 10 msec ) 刷新。在每個周期內的細胞，只能感知到上個週期身邊細胞的狀態。  
例如，在 300ms ~ 310ms 時段內，A 細胞看到她周圍細胞狀態，會是 290ms ~ 300ms 期間內的結果。A 細胞在他自身週期到的時候，改變的狀態，其他細胞要在下個週期 ( 310ms ~ 320ms ) 才能感知的到。
1. 如果外界環境週期固定為 10ms, 所有細胞從 5ms 開始，周期也固定為 10ms, 那麼所有的表現都應該跟 milestone 2 一樣才對。

> 這個題目我把規則定義清楚了，不過我還沒想到怎麼驗證... XDD, 單元測試等我想出來就會補上了 :D

## Milestone 4, 大亂鬥

如果你以上三個里程碑都達標的話，我會釋出我的 Milestone 4 題目 (有標準定義的 ICell interface 需要實作)。我將會在相同限制條件下，允許每個參賽者調整部分的參數，一同放到同一個培養皿裡面生活。看看多個世代下去，哪個物種能夠佔據最多的數量? 你無法預測你的競爭對手參數會怎麼調整，只能自己想辦法提高求生機率。

> 題目還沒想好怎麼開，敬請期待 XDD



