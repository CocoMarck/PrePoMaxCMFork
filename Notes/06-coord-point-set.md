# CoordPointSet
### `2026-05-28`
Este objeto tiene datos serializados almacena `CoordPoint`. Guarda de una coordenadas repetidas. No le importa que se repitan, lo unico que no repite son ids. Tiene que ser un CoordPointSet generico, por lo que evitar puntos de coordenadas duplicadas, no debe ser pioridad.

Se podria hacer que no repita coords, pero no se si sea necesario. Capaz mejor que la GUI lo haga... Pero medio raro si es asi.

El incremento de ids debe ser acomulativo. Por lo que hay que tenerlo en cuenta si se quiere evitar puntos repetidos.

Actualmente obtengo ids por `Controller.Model.Mesh.GetNextPointId();`. En el mesh se acomula eso.

De hecho esto es un problema, porque de uno almacena ids. Sin importar el set. Todos se acomulan, es decir set hola, almacena 4 puntos, y luego se crea el set adios, y este almacena tambien 4. Tons hola tiene de ids, 1, 2, 3, 4. Y Adios almacena de ids 5, 6, 7, 8.

GetNextId, en realidad solo un acumulador del total de ids.

Dilema resuelto: para un set de ids, ids contados por set va bien. Pero pa varias, hay problemas. Lo mejor es mantener simple todo lo relacionado a CoordPointSet, y caundo se guarden los puntos con su id, pos un count, un foreach, y ya ta. Y cuando quiera todos los sets de una, pos nadota, nomas obtengo rodos los sets, y pongo directo sus ids. Ademas se asume que medio raro si haces nomas 10 trayectorias, y nomas usas una. Puede pasar, pero es raro.

Mantener simplicidad. Dejar todo igual.