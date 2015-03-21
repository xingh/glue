# Open() and Close() #

`Glue.Data.IDataProvider.Open();`<br />
`Glue.Data.IDataProvider.Close();`

Normally, you shouldn't have to worry about opening and closing connections, because the DataProvider will do it automatically. A connection is created when the first query is executed and a new transaction is started. The connection is closed automatically when the DataProvider is disposed of.

Closing a DataProvider also commits any changes.

# Cancel() #

`Glue.Data.IDataProvider.Cancel();`

Cancel() rolls back the transaction (if one exists).

**Warning**: check if transactions work for your DataProvider. Not all types of provider have transaction support built in (yet).

# Transactions #

Open() returns a cloned DataProvider that starts a new transaction. You can use this DataProvider to group queries in one transaction.

**Warning**: check if transactions work for your DataProvider. Not all types of provider have transaction support built in (yet).

```
using (IDataProvider provider = Global.DataProvider.Open())
{
    try
    {
        // Mutate() changes the bank balance by an amount
        debtor.Mutate(-100);
        creditor.Mutate(100);
        provider.Update(debtor);   // may fail for some reason (insufficient funds)
        provider.Update(creditor); // may fail for some other reason (account is frozen)
    }
    catch (Exception e)
    {
        provider.Cancel(); // Rollback transaction
        throw e;
    }
}
```
_Example: updating a bank account balance in a transaction._