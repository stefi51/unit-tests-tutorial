# Pisanje jediničnih (Unit) testova u .NET projektima uz pomoć [Moq](https://github.com/devlooped/moq), [Fluent Assertions](https://fluentassertions.com/) biblioteka

---
### Šta predstavlja Unit testiranje u softverskom inženjerstvu?
Unit testiranje predstavlja proces testiranja najmanjih funkcionalnih delova(units) softverske aplikacije, tipično su to funkcije ili metode, gde je primarni cilj potvrditi da one ispravno i očekivano funkcionišu. <br>
Unit testiranja omogućava rano otkrivanje grešaka(bugs) tokom samog razvoja funkcionalnosti aplikacije, zatim poboljšanje kvaliteta nastalog koda, kao i smanjenje vremena za testiranje novonastalih izmena tokom ciklusa održavanja softverske aplikacije. <br>

Osnovni je deo **Test-Driven Development (TDD)**, gde se Unit testovi, sa očekivanim rezultatima koji unit treba da zadovolji, pišu pre same implementacije unit-a.<br> Ovo predstavlja ekstremni slučaj, u praksi većina softver inženjera piše Unit testove nakom same implementacije da bi testirali novu funkcionalnost i pre samog kreiranja pull-request-a(PR).

//TODO prednosti i mane

---
У овом пројекту коришћена је MSTest библиотека за јединично тестирање. Поред ње познате су и xUnit и NUnit.

---
Mock библиотека: У овом пројекту коришћена је Moq библиотека.
Поред њих су познате и јако добре [NSubstitute](https://nsubstitute.github.io/) и [FakeItEasy](https://fakeiteasy.github.io/) .
Више о њима може се наћи у њиховој званичној документацији. 

---
Assertion библиотека: У овом пројекту коришћена је [Fluent Assertions](https://fluentassertions.com/) библиотека. 
Поред ње на располагању је и [Shouldly](https://docs.shouldly.org/) библиотека.