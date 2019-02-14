# What can this thing do? And how can I do it?

Control database transactions inside your C# code (no more transactions inside big procedures).
Just add the Calmo.Core.Data namespace whenever you need it.

It's very simple and straightforward:

```
using(var scope = new TransactionScope)
{
    // Just call your repositories inside the scope, that's it: 
    // You now have a transaction, if any call inside the scope throws an exception, you're safe.
    repository.Insert(entity);
    repository2.Update(otherEntity)
    ...
    // In the end, call the complete do commit the transaction,
    // otherwise on dispose the TransactionScope will call a rollback 
    scope.Complete()
}
```

