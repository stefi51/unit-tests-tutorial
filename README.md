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
}
```
Kao što se može videti iz primera, ovaj Unit test će izvršiti 2 test scenaria, za količinu od 2,3 i popustom od 10% i 20% procenata.<br>
👉 [Source kod se nalazi u Concepts.cs file-u. (lines 33–48)](https://github.com/stefi51/unit-tests-tutorial/blob/main/tests/Template.Business.UnitTests/Concepts.cs#L33-L48)

---

У овом пројекту коришћена је MSTest библиотека за јединично тестирање. Поред ње познате су и xUnit и NUnit.

---
Mock библиотека: У овом пројекту коришћена је Moq библиотека.
Поред њих су познате и јако добре [NSubstitute](https://nsubstitute.github.io/) и [FakeItEasy](https://fakeiteasy.github.io/) .
Више о њима може се наћи у њиховој званичној документацији. 

---
Assertion библиотека: У овом пројекту коришћена је [Fluent Assertions](https://fluentassertions.com/) библиотека. 
Поред ње на располагању је и [Shouldly](https://docs.shouldly.org/) библиотека.