open Feliz
open Browser.Dom
open System

let calculateWinner (squares: string[]) =
    let lines = [|
        [| 0; 1; 2 |]
        [| 3; 4; 5 |]
        [| 6; 7; 8 |]
        [| 0; 3; 6 |]
        [| 1; 4; 7 |]
        [| 2; 5; 8 |]
        [| 0; 4; 8 |]
        [| 2; 4; 6 |]
    |]

    let mutable result = ""
    for i = 0 to lines.Length - 1 do
        // let [| a; b; c |] = lines.[i]
        match lines.[i] with
        | [| a; b; c |] ->
            if (squares.[a] <> "") && (squares.[a] = squares.[b]) && (squares.[a] = squares.[c]) then
                result <- squares.[a]
        | _ ->
                raise (Exception("unexpected length of row")) 
    result

[<ReactComponent>]
let Square (value: string) onSquareClick =

    Html.button [
        prop.className "square"
        prop.text value
        prop.onClick (fun _ -> onSquareClick ())
    ]

[<ReactComponent>]
let Board xIsNext squares onPlay =

    let winner = calculateWinner squares
    let status = 
        if winner <> "" then
            "Winner: " + winner
        else
            "Next player: " + (if xIsNext then "X" else "O")

    let handleClick i =
        match winner with
        | "" ->
            match squares.[i] with
            | "" ->
                let nextSquares = Array.copy squares
                if xIsNext then
                    nextSquares.[i] <- "X"
                else
                    nextSquares.[i] <- "O"
                onPlay nextSquares
            | _ -> ()
        | _ -> ()

    Html.div [
        Html.div [
            Html.text status
        ]
        Html.div [
            prop.className "board-row"
            prop.children [
                Square squares.[0] (fun _ -> handleClick 0)
                Square squares.[1] (fun _ -> handleClick 1)
                Square squares.[2] (fun _ -> handleClick 2)
            ]
        ]
        Html.div [
            prop.className "board-row"
            prop.children [
                Square squares.[3] (fun _ -> handleClick 3)
                Square squares.[4] (fun _ -> handleClick 4)
                Square squares.[5] (fun _ -> handleClick 5)
            ]
        ]
        Html.div [
            prop.className "board-row"
            prop.children [
                Square squares.[6] (fun _ -> handleClick 6)
                Square squares.[7] (fun _ -> handleClick 7)
                Square squares.[8] (fun _ -> handleClick 8)
            ]
        ]
    ]

[<ReactComponent>]
let Game () =

    let (history, sethistory) = React.useState([| [| for _ in 1..9 -> "" |] |])
    let (currentMove, setCurrentMove) = React.useState(0)
    let currentSquares = history.[currentMove]
    let xIsNext = currentMove % 2 = 0

    let handlePlay nextSquares =
        let nextHistory = Array.append history[0..currentMove] [| nextSquares |]
        sethistory nextHistory
        setCurrentMove(nextHistory.Length - 1);

    let jumpTo (nextMove: int) =
        setCurrentMove nextMove

    let moves = 
        Array.mapi (fun idx _ ->
            let description =
                if idx > 0 then
                    "Go to move #" + idx.ToString()
                else
                    "Go to game start"
            Html.li [
                prop.key idx
                prop.children [
                    Html.button [
                        prop.onClick (fun _ -> jumpTo idx)
                        prop.text description
                    ]
                ]
            ]
        ) history

    Html.div [
        prop.className "game"
        prop.children [
            Html.div [
                prop.className "game-board"
                prop.children [
                    Board xIsNext currentSquares handlePlay
                ]
            ]
            Html.div [
                prop.className "game-info"
                prop.children [
                    Html.ol moves
                ]
            ]
        ]
    ]


let root = ReactDOM.createRoot (document.getElementById "root")
root.render (Game())
