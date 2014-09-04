#light 
namespace  Comet.Chating

open System
open System.Linq
open MongoDB
open MongoDB.Linq 
open System.Collections.Concurrent
open Microsoft.FSharp.Collections
module public  Chat =
    type ChatMsg = {
        From: string;
        Text: string;
    }
    type ChatMan = {
        Room: string;
        Name: string;
    }    

    let private agentCache = new ConcurrentDictionary<ChatMan, MailboxProcessor<ChatMsg>>()

    let private agentFactory = new Func<ChatMan, MailboxProcessor<ChatMsg>>(fun _ -> 
        MailboxProcessor.Start(fun o -> async { o |> ignore })) 
    
    let private GetAgentRoom man =    agentCache.GetOrAdd(man  , agentFactory)   
    let private GetMan name room = {Room=room;Name=name}   
    let send fromName toName msg room  =
        let man = GetMan toName  room  
        let agent = GetAgentRoom man
        { From = fromName; Text = msg; } |> agent.Post
  
    
    let sendall fromName  msg  room =    
          async { 
             agentCache 
             |>PSeq.filter(fun ac->ac.Key.Room.Equals room)|>PSeq.iter(fun (a)->{ From = fromName; Text = msg; } |> a.Value.Post ) 
               
          }
    let sendallwithoutadmin fromName  msg  room =    
          async { 
             agentCache 
             |>PSeq.filter(fun ac->ac.Key.Room.Equals room && not(ac.Key.Name.Equals "admin"))|>PSeq.iter(fun (a)->{ From = fromName; Text = msg; } |> a.Value.Post ) 
               
          }
          
  

    let receive name room= 
        let man = GetMan name  room
        let rec receive' (agent: MailboxProcessor<ChatMsg>) messages = 
            async {
                let! msg = agent.TryReceive 0
                match msg with
                | None -> return messages
                | Some s -> return! receive' agent (s :: messages)
            }

        let agent = GetAgentRoom man

        async {
            let! messages = receive' agent List.empty
            if (not messages.IsEmpty) then return messages
            else
                let! msg = agent.TryReceive 3000
                match msg with
                | None -> return []
                | Some s -> return [s]
        }