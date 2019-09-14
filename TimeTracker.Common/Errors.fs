namespace TimeTracker.Common

module Errors =
    
    type Error =
    | ValidationError of string
    | DatabaseError of string




