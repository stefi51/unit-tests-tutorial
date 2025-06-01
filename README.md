# Pisanje jediničnih (Unit) testova u .NET projektima uz pomoć [Moq](https://github.com/devlooped/moq), [Fluent Assertions](https://fluentassertions.com/) biblioteka

---
### Šta predstavlja Unit testiranje u softverskom inženjerstvu?
Unit testiranje predstavlja proces testiranja najmanjih funkcionalnih delova(units) softverske aplikacije, tipično su to funkcije ili metode, gde je primarni cilj potvrditi da one ispravno i očekivano funkcionišu. <br>
Unit testiranja omogućava rano otkrivanje grešaka(bugs) tokom samog razvoja funkcionalnosti aplikacije, zatim poboljšanje kvaliteta nastalog koda, kao i smanjenje vremena za testiranje novonastalih izmena tokom ciklusa održavanja softverske aplikacije. <br>

Osnovni je deo **Test-Driven Development (TDD)**, gde se Unit testovi, sa očekivanim rezultatima koji unit treba da zadovolji, pišu pre same implementacije unit-a.<br> Ovo predstavlja ekstremni slučaj, u praksi većina softver inženjera piše Unit testove nakom same implementacije da bi testirali novu funkcionalnost i pre samog kreiranja pull-request-a(PR).

TODO prednosti i mane

---

### Primer Unit test i AAA (Arrange, Act, Assert) obrazac

Unit test predstavlja blok koda koji verifikuje ispravnost izolovanog unita.<br>
Napisan je tako da verifikuje da li se određeni unit ponaša u skladu sa željenim ponašanjem koje je softver inženjer zahteva.<br>
Obrazac koji je skoro postao standard u industriji i često se sreće, AAA (Arrange, Act and Assert), ima za ideju da se Unit test podeli u tri faze i to:<br>
- Arrange( Aranžirati ) deo u kom se pripremaju ulazni podaci, mock-uju zavisnost ako ih Unit poseduje, i eventualno istancira Unit.
- Act (Postupak) deo u kom se izvršava Unit koji se testira.
- Asssert (Tvrditi) deo u kom se proverava da li rezultat izvršenja zadovoljava definisane kriterijume.

Primer Unit testa koji testira izračunavanje konačne cene narudzbine sa primenjenim popustom:
```csharp
[TestMethod]
[DataRow(100.0, 2, 10.0, 180.0)]
[DataRow(100.0, 3, 20.0, 240.0)]
public void CalculateTotal_ShouldApplyDiscountCorrectlyMultiple(double price, int quantity,
                                                                double discountPercent, double expectedTotal)
{
    // Arrange    
    var service = new OrderService();
    
    // Act       
    var totalPrice = service.CalculateTotal(price: (decimal)price, quantity: quantity,
                                                    discountPercent: (decimal)discountPercent);

    // Assert    
    Assert.AreEqual((decimal)expectedTotal, totalPrice);
    // 200 - 10% = 180
    // 300 - 20% = 240
}
```
👉 [Source kod se nalazi u Concepts.cs file-u. (lines 33–48)](https://github.com/stefi51/unit-tests-tutorial/blob/main/tests/Template.Business.UnitTests/Concepts.cs#L33-L48)
<br>
Kao što se može videti iz primera, ovaj Unit test će izvršiti za 2 test scenaria, za količinu od 2,3 i popustom od 10% i 20% procenata.<br>
Ako metoda CalculateTotal vrati očekivane rezultate, test će biti označen kao uspešan(passed), ako rezultat bude različit u odnosu na to šta je očekivano, u ovom slučaju 180 i 240, test će biti označen kao neuspešan (failed). <br>

---
### Izbor framework-a za Unit testiranje
Za izradu ovog tutorijala korišćena je [MSTest](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-mstest) framework za testiranje.<br> Prvenstveno jer dolazi već integrisan tokom kreiranja projekta, i zbog jednostavne sitankse koja je poznata većem broju ljudi (TestMethod, TestClass). <br>
Pored MSTest biblioteke poznate su i široko rasporstanje su :
- [xUnit](https://xunit.net/), otvorenog koda, inicijalno kompleksnija sintaksa, zahteva krivu učenja, pogodna za .NET Core i NET 5 i novije verzije.
- [NUnit](https://nunit.org/) , otvorenog koda, pogodna za velike projekte sa puno testova.
<br>

  | Feature                          | **MS Test**                         | **NUnit**                    | **xUnit**                         |
  | -------------------------------- | ----------------------------------- | ---------------------------- | --------------------------------- |
  | **Test Class Attribute**         | `[TestClass]`                       | `[TestFixture]` *(optional)* | ❌ No class attribute needed       |
  | **Test Method Attribute**        | `[TestMethod]`                      | `[Test]`                     | `[Fact]`                          |
  | **Parameterized Test Attribute** | `[DataTestMethod] + [DataRow(...)]` | `[TestCase(...)]`            | `[Theory] + [InlineData(...)]`    |
  | **Setup Method**                 | `[TestInitialize]`                  | `[SetUp]`                    | Constructor or `IClassFixture<T>` |
  | **Teardown Method**              | `[TestCleanup]`                     | `[TearDown]`                 | `IDisposable.Dispose()`           |
  | **Assert Class**                 | `Assert.AreEqual(...)`              | `Assert.AreEqual(...)`       | `Assert.Equal(...)`               |

---
Mock библиотека: У овом пројекту коришћена је Moq библиотека.
Поред њих су познате и јако добре [NSubstitute](https://nsubstitute.github.io/) и [FakeItEasy](https://fakeiteasy.github.io/) .
Више о њима може се наћи у њиховој званичној документацији. 

---
Assertion библиотека: У овом пројекту коришћена је [Fluent Assertions](https://fluentassertions.com/) библиотека. 
Поред ње на располагању је и [Shouldly](https://docs.shouldly.org/) библиотека.