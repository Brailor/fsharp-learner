//TODO: how to auto format f# files
module App

open Elmish
open Elmish.Debug
open Elmish.React
open Elmish.Navigation
open Elmish.UrlParser
open Fable.React.Props
open Fable.React.Helpers
open Fable.React.Standard
open Fable.SimpleHttp

type Route = Counter | Users
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
        route : Route 
    }

type Service = 
    { getUsers : unit -> Async<User> }

type Msg =
    | Increment
    | Decrement
    | GetUsers
    | GotUsers of string
    | Error of exn

//TODO: investigate how to do this more elegantly
let remote _ =
    async {
        let! (statusCode, (responseText)) = Http.get "/weatherforcast"

        match statusCode with
                | 200 -> printfn "Everything is fine => %s" responseText
                | _ -> printfn "Status %d => %s" statusCode responseText

        return responseText
    }

let route = 
        oneOf 
            [  UrlParser.map Counter (UrlParser.s "counter");
             UrlParser.map Users (UrlParser.s "users")]


let increment x = x + 1
let decrement x = x - 1

let initModel = 
    { 
        x = 0
        users = None
        errors = ""
        route = Counter
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

let urlUpdate (result: Option<Route>) model = 
    match result with
    | Some page ->
        { model with route = page }, []
    | None -> 
        { model with route = Counter }, []

let counterView model dispatch =
     div [] [
        div [] [ a [ Href "#users"] [ str "Users" ] ]
        button [ OnClick (fun _ -> dispatch Decrement) ] [ str "-" ]
        span [] [ str (model.x.ToString()) ]
        button [ OnClick (fun _ -> dispatch Increment) ] [ str "+" ] ]

let usersView model =
     div [] [
          div [] [ a [ Href "#counter"] [ str "Counter" ] ]
          str (sprintf "%A" model.users) ]
    
let view model dispatch =
     div [] [
        match model with
            | { route = Counter } -> 
                div [] [ counterView model dispatch ]
            | { route = Users } ->
                div [] [ usersView model ] ]


Program.mkProgram (fun _ -> initModel) update view
|> Program.toNavigable (parseHash route) urlUpdate
|> Program.withReactBatched "elmish-app"
|> Program.withDebugger
|> Program.run
