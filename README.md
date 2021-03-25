# XbimSamples

Tech Stac : .NET 5 Web Api

Download/Clone the project and run it

You should be able to see url like this https://localhost:44381/swagger/index.html

Since its swagger enabled, you can see 2 end points

1. /ElementTypeCount
2. /RoomArea

both has 'Try it out' button, clicking on it opens panel where you can click 'Execute' button, as both endpoints doesnt take parameters, you should see the responses from the model file

![image](https://user-images.githubusercontent.com/29385156/112517960-d5ff0880-8d90-11eb-8e76-0e85d705f6be.png)


![image](https://user-images.githubusercontent.com/29385156/112517847-b7990d00-8d90-11eb-989b-1c83b48847f4.png)


Areas to Refactor:

1.Create service class, and its interface
2.Create repo class, and its interface
3.Write unit tests
4.Add more logging
5.Wireup DI in startup class
6.Add Authentication
