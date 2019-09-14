namespace TimeTracker.Common

module Hashing =
    
    open System.Text
    open System.Security.Cryptography

    let private AppendHashBuilder (builder:StringBuilder) (byte:byte) =
        builder.AppendFormat("{0:X2}", byte)

    let GeneratePasswordHash (password:string) =
        let passwordBytes = Encoding.UTF8.GetBytes(password)
        let sha = SHA1.Create()
        let hash = sha.ComputeHash(passwordBytes) |> sha.ComputeHash
        hash |> Array.fold AppendHashBuilder (StringBuilder("*")) 
             |> fun x -> x.ToString()
