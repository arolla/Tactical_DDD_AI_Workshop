# Tactical DDD Refactoring Diagnostic Flow

Use this decision tree to evaluate and refactor domain components (**Service**, **Entity**, **Value Object**, **Repository**) iteratively. Follow the branches based on the current state of the code.

---

## 3. Value Object

* **Do you have setter?**
  * **No:** Great!
  * **Yes:** Your Value Object is not immutable; refactor to delete the setter.

* **Do you have a parameterized constructor?**
  * **No:** You probably have setters. *(-> Go to "Do you have setter?")*
  * **Yes:** **Do you have more than one?**
    * **No:** Great!
    * **Yes:** **Do you use different constructors to manage default values? Do all the constructors call the same one?**
      * **Yes:** Perhaps you can use default parameters instead.
      * **No:** You probably need to split your Value Object into many.
  * **Yes (continued):** **Do you have any data validations inside the constructor?**
    * **No:** Check that the validation is not outside your Value Object (before calling the constructor). Bring the validations inside your Value Object and use a factory method to validate inputs and create the instance.
    * **Yes:** Extract them and use a factory method to validate inputs and create the instance.

* **Do you have getter?**
  * **No:** Great!
  * **Yes:** Find out where the getter is used, bring the behavior inside your Value Object, and delete the getter (*Tell, Don't Ask* principle).

* **Do you have method?**
  * **No:** You probably have getters, so go to *"Do you have getter?"*
  * **Yes:** **Do you have any parameter?**
    * **No:** Make sure that there is no message obsession.
    * **Yes:** **Do you have more than 4 parameters?**
      * **No:** Great!
      * **Yes:** You probably need to encapsulate some parameters into a Parameter Object (Value Object).

* **Are the attributes read-only/final?**
  * **No:** **Do you need to change their values?**
    * **No:** Make them read-only/final.
    * **Yes:** You probably need an Entity instead of a Value Object. *(-> Evaluate as Entity)*
    
---

## 2. Entity

* **Do the attributes' values change over time while the identity remains the same?**
  * **Yes:** Fine!
  * **No:** Perhaps it should be a Value Object. *(-> Evaluate as Value Object)*

---

## 1. Service

* **Do you use any SQL transaction?**
  * **No:** Great!
  * **Yes:** Create an Aggregate for the cluster ("grape") and move the transaction into the Repository of this Aggregate. *(-> See Repository)*

* **Do you have any static method?**
  * **No:** Fine!
  * **Yes:** The behavior should be moved into one of the parameters (an Entity or a Value Object).

* **Do its methods use any field?**
  * **No:** The behavior should be moved into one of the parameters.
  * **Yes:** Fine!

* **Do you have any dependency?**
  * **No:** It should be an Entity or a Value Object instead. *(-> Evaluate as Entity or Value Object)*
  * **Yes:** **Do you have many repositories as dependencies?**
    * **No, just one:** Fine!
    * **Yes:** Create an Aggregate for the cluster and move the transaction/coordination into the Repository of this Aggregate. *(-> See Repository)*

---

## 4. Repository

* **Do you have a repository by entity or value object?**
  * **Yes:** That means that your domain is a CRUD one (no problem); otherwise check your Aggregates.

* **Do you have any interaction with a database?**
  * **Yes:** **Do you need any transaction?**
    * **Yes:** Check if the transaction is not in the caller of this method/instance.
  * **No:** **Do you use an external service?** *(-> Go to "Do you use external service?")*

* **Do you use external service?**
  * **No:** Fine!
  * **Yes:** **Do you need to be conformist to that service?**
    * **No:** Ensure that the Anti-Corruption Layer (ACL) exists.

* **Do you have any instruction which seems like a behaviour? (addition, verification...)**
  * **Yes:** Move it into the domain code (Value Object, Entity, Service).
