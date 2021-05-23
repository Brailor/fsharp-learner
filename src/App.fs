//TODO: how to auto format f# files
module App

open Elmish
open Elmish.React
open Fable.React.Props
open Fable.React.Helpers
open Fable.React.Standard
open Fable.SimpleHttp


type User = 
    {
        id : string
        name : string
        username : string
        email: string
    }

type Model = 
    { 
        x : int
        users : string option
        errors : string 
    }

type Service = 
    { getUsers : unit -> Async<string> }

type Msg =
    | Increment
    | Decrement
    | GetUsers
    | GotUsers of string
    | Error of exn

//TODO: investigate how to do this more elegantly
let remote _ =
    async {
        let! (statusCode, (responseText)) = Http.get "https://jsonplaceholder.typicode.com/users/123213s"

        match statusCode with
                | 200 -> printfn "Everything is fine => %s" responseText
                | _ -> printfn "Status %d => %s" statusCode responseText

        return responseText
    }

let increment x = x + 1
let decrement x = x - 1

let initModel = 
    { 
        x = 0
        users = None
        errors = ""
    }, Cmd.ofMsg GetUsers

let update msg model = 
    match msg with
    | Increment ->
        { model with x = increment model.x }, Cmd.none
    | Decrement ->
        { model with x = decrement model.x }, Cmd.none
    | GetUsers ->
       let cmd = Cmd.OfAsync.either remote () GotUsers Error
       model, cmd
    | GotUsers users ->
        { model with users = Some users}, Cmd.none
    | Error e ->
        { model with users = None; errors = e.ToString() }, Cmd.none

    
let view model dispatch =
     div []
      [ button [ OnClick (fun _ -> dispatch Decrement) ] [ str "-" ]
        button [ OnClick (fun _ -> dispatch Increment) ] [ str "+" ] 
        div [] [ str (sprintf "%A" model) ] ]


Program.mkProgram (fun _ -> initModel) update view
|> Program.withReactBatched "elmish-app"
|> Program.run
