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
### Problemi i izazovi pri pisanju Unit testova u realnim projektima
// TODO prvi problem <br>
Jedan od većih izazova u pisanju Unit testova je kako izolovati unit koji se testira od modula i servisa od koje on zavisi i da sam rezultat izvršenja našeg unit testa ne zavisi od spoljnih uticaja, to jest od zavisnih servisa(dependencies). <br>
Jedno od resenja je koristiti Dependency Injection (DI) softverski obrazac. <br>
Na ovaj način je moguće u testu napraviti objekat koji simulira servis od koga sam unit zavisi, međutim to zahteva dodatan posao gde bi softver inženjer morao napisati implemtaciju za simulaciju. 
Ovaj koncept poznaz je kao mocking u Unit testiranju i sam framework za Unit testiranje to ne podržava podrazumevano, zato je potrebno koristiti dodatne bibilioteke koje to omogućavaju za nas, kao što su:<br>
- [Moq](https://github.com/devlooped/moq)
- [NSubstitute](https://nsubstitute.github.io/)
- [FakeItEasy](https://fakeiteasy.github.io/)

Mocking servisa od koje Unit zavisi se obično piše u Arrange fazi ili u [TestInitialize] metodi ako se potrebno koristiti mock servis u više test case-va.
U ovom tutorijalu korišćena je **Moq** biblioteka jer se jedna od napoznatijih i često korišćenijih u industriji iako je njena sintaksa možda robusnija od konkurentskih biblioteka.

#### Primer metode koja zavisi od third-part servisa i baze podataka koja se može isto tretirati kao dependency
```csharp
    public async Task DeleteUser(Guid userId)
    {
        // database, takođe dependency
        var user = await _repository.Query<User>().SingleOrDefaultAsync(x => x.Id == userId);

        if (user is not null)
        {
            // third party service
            var hasPendingPayments = await _paymentService.HasPendingPayments(user.Email);
            if (hasPendingPayments)
            {
                throw new Exception($"User with email:{user.Email} has pending payments");
            }
            _repository.Delete(user);
            _repository.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"User with UserId:{userId} does not exist.");
        }
    }
```
👉 [Source code (lines 33–48)](https://github.com/stefi51/unit-tests-tutorial/blob/main/src/Template.Business/Services/UserService.cs#L53-L72)
<br>
Da bi napisali dobre unit testove za gore navedenu metodu potrebno napraviti mock objekte koji će simulirati izvršenje stvarnih servisa.<br>
U ovom slučaju flow koji treba pokriti je:
1. User nije pronađen u bazi podataka
 - Mock treba izgledati ovako:
```csharp
        // Arrange    
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>(){ }.AsQueryable().BuildMock());
        // Simulacija prazne baze podataka
```
2. User je pronađen u bazi podataka i nema neizvršenih plaćanja
- Mock treba izgledati ovako:
```csharp
        // Arrange
        var userId = Guid.NewGuid();
        var email = "john.doe@test.com";
        // Simulacija da user postoji u bazi podataka
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>()
        {
            new()
            {
                Id = userId,
                Email = email,
            },
        }.AsQueryable().BuildMock());
        // Simulacija third-part servisa, gde on vraća da user sa ovim email-om nema neizvršenih plaćanja
        _paymentService.Setup(x => x.HasPendingPayments(email)).ReturnsAsync(false);

```
3. User je pronađen ali ne može biti obrisan zato što ima neizvršena plaćanja
- Mock treba izgledati ovako:
```csharp

        // Arrange
        var userId = Guid.NewGuid();
        var email = "john.doe@test.com";
        // Simulacija da user postoji u bazi podataka
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>()
        {
            new()
            {
                Id = userId,
                Email = email,
            },
        }.AsQueryable().BuildMock());
         // Simulacija third-part servisa, gde on vraća da user sa ovim email-om ima neizvršenih plaćanja
        _paymentService.Setup(x => x.HasPendingPayments(email)).ReturnsAsync(true);
```
<br>

---
##### Problem sa pisanjem tvrdnji (Assert)
Naredni čest problem koji se javlja kod pisanja Unit testova vezan je za pisanje samih tvrdnji koje rezultat izvršenja i Unit test treba da zadovolji. <br>
Sam framework za Unit testiranje pruža Assert metode ali one obično rade sa prostim tipovima podataka i pisanje kompleksnih tvrdnji zahteva puno više linija koda. <br>
Takođe pisanje tvrdnji koje bi obuhvatale testiranje objekata ili listi objekata bez biblioteka bi zahtevalo testiranje svakog property-a zasebno, što može biti zamorno i nečitljivo. <br>
Da bi se rešio ovaj problem na raspologanju su nam biblioteka koja omogućavaju jednostavnije i lako čitljive tvrdnje. <br>

U ovom tutorijalu korišćenja je [Fluent Assertions](https://fluentassertions.com/) biblioteka, pored nje poznata je i često korišćena je i [Shouldly](https://docs.shouldly.org/) biblioteka. <br>

Za korišćenje Fluent Assertions biblioteke potrebno je instalirati je u projekat u kome se nalaze testovi pomoću Package menadžera ili komandom:
```
dotnet add package FluentAssertions
```

##### Primer pisanja tvrdnji sa Fluent Assertions bibliotekom sa validacijom objekata i stringova
```csharp
    [TestMethod]
    public async Task GetUserByUidWhenUserExists()
    {
        //Arrange
        var user = new UserDto()
        {
            UserUid = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd4"),
            LastName = "Doe",
            Name = "John",
            Email = "john.doe@test.com"
        };

        //Act
        var result = await _sut.GetUser(user.UserUid);

        //Assert

        // Sa Fluent Assertion bibliotekom
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);

        // Bez Fluent Assertion biblioteke
        /*Assert.IsNotNull(result);
        Assert.AreEqual(result.UserUid, user.UserUid);
        Assert.AreEqual(result.Email, user.Email);
        Assert.AreEqual(result.Name, user.Name);*/
        
        // Sa Fluent Assertion bibliotekom
        user.Name.Should().NotBeNull().And.StartWith("J").And.EndWith("n");

        // Bez Fluent Assertion biblioteke
        /*Assert.IsNotNull(user.Name);
        Assert.IsTrue(user.Name.StartsWith("J"));
        Assert.IsTrue(user.Name.EndsWith("n"));*/
  
    }
```
👉 [Source code (lines 62–95)](https://github.com/stefi51/unit-tests-tutorial/blob/main/tests/Template.Business.UnitTests/UserServiceUnitTests.cs#L62-L95)
<br>
Na gore navedenom primeru prikazana je komparacija sa korišćenjem i bez korišćenja Fluent Assertions biblioteke, korišćenje biblioteke smanjuje pisanje samog koda i pojednostavljuje komparaciju objekata. <br>
Sama komparacija će se izvršiti nad svakim property-em samog objekta.
<br>
##### Primer pisanja tvrdnji sa Fluent Assertions bibliotekom i validacijom liste objekata
```csharp
    [TestMethod]
    public async Task GetUsers()
    {
        //Arrange
        var expectedUser1 = new UserDto()
        {
            UserUid = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd4"),
            LastName = "Doe",
            Name = "John",
            Email = "john.doe@test.com"
        };
        var expectedUser2 = new UserDto()
        {
            Name = "Mark",
            LastName = "Cooper",
            Email = "mark.cooper@test.com",
            UserUid = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd5")
        };

        var expectedUsers = new List<UserDto>()
        {
            expectedUser1,
            expectedUser2
        };

        // Act
        var users = await _sut.GetUsers();

        //Assert
        users.Should().NotBeNull();
        users.Should().BeEquivalentTo(expectedUsers);
    }
```
---
#### Još neke od tehnika pri pisanju tvrdnji

Još jedna korisna mogućnost koju nam pruža Moq biblioteka a može biti korisno kod pisanja tvrdnji je da li je neka funkcija pozvana tokom izvršenja i koliko puta. <br>

```csharp
    [TestMethod]
    public async Task DeleteUserHappyPath()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "john.doe@test.com";
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>()
        {
            new()
            {
                Id = userId,
                Email = email,
            },
        }.AsQueryable().BuildMock());
        _paymentService.Setup(x => x.HasPendingPayments(email)).ReturnsAsync(false);

        //Act
        await _sut.DeleteUser(userId);

        // Assert
        _repository.Verify(repo => repo.Delete(It.Is<User>(u => u.Id == userId
                                                                && u.Email == email)), Times.Once);
        _repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _paymentService.Verify(payment => payment.HasPendingPayments(email), Times.Once);
    }
```
Na ovom primeru može se videti tvrdnja da metoda DeleteUser() treba pozvati svaku od navedenih metoda po 1 put, ako to nije slučaj ili ulazni podaci ne odgovaraju (userId i Email), test će rezultirati kao failed. <br>

Takođe moguće je i testirati da li desio očekivani izuzetak(Exception) tokom izvršenja.
```csharp
    [TestMethod]
    public async Task DeleteUserWhenUserDoesNotExist()
    {
        // Arrange    
        var userId = Guid.NewGuid();
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>(){ }.AsQueryable().BuildMock());

        // Act   
        var requestAction = async () => await _sut.DeleteUser(userId);

        // Assert 
        await requestAction.Should().ThrowAsync<Exception>()
            .WithMessage($"User with UserId:{userId} does not exist.");

        _repository.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Never());
        _repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _paymentService.Verify(payment => payment.HasPendingPayments(It.IsAny<string>()), Times.Never);
    }
```
---