# Compilar PrePoMax con Visual Studio `2026`
> Nota: PrePoMax necesita muchas libs muy dependientes de Windows, no jala en Linux.

**Se necesita una computadora de 64 bits**

### Programas/Dependencias necesarias
- Primeramente instalar [Visual Studio Comunity](https://www.visualstudio.com/downloads/).
    - Seleccionamos `NET C# Desktop Development`.
    - Seleccionamos `Desktop development with C++`.

- Despues instalar ActiViz OpenSource Edition 5.8.0. [PrePoMax Downloads](https://prepomax.fs.um.si/downloads/)

> NET C# debe incluir `GmshCommon`, y lo de C++, cosas de compilación del solucionador del PrePoMax.

### Source code
- Clonamos con git, o nomas descargamos [source de code](https://gitlab.com/MatejB/PrePoMax). La rama `master`.

> Leer instrucciones del README, las actualizan a cada rato.

---
## Compilar con Visual Studio
Abrimos la sulución `PrePoMax\PrePoMax.sln`.

### Recreamos las librerias de VTK
#### Elminar las dos referencias feas a VTK Library
- Abrimos el `Solution Explorer`: View -> Solution Explorer.
- En el `Solution Explorer` buscamos el proyecto `vtkControl` y buscamos las referencias feas.
- Elimina todas las referencias que empiezen con `Kitware` (click derecho y borrar).

#### Agregar libs/referencias a vtkControl
- Click derecho en `References` del proyecto `vtkControl` en la ventana `Solution Explorer`, agregamos referencia.
- Buscamos el archivo `C:\Program Files\ActiViz.NET 5.8.0 OpenSource Edition\bin\Kitware.VTK.dll`, y lo agregamos.
- Buscamos el archivo `C:\Program Files\ActiViz.NET 5.8.0 OpenSource Edition\bin\Kitware.mummy.Runtime.dll`, y lo agregamos.

> Las librerias que buscamos, no existiran si no instalamos **ActiViz OpenSource Edition 5.8.0**

### Retarget Solution
Finalmente un `Retarget Solution` a todo el proyecto, desde `Solution Explorer`, y darle Check a todas las opciones, y aplicar.

---
## Nota
**Mejor clonar con Git que descargar ZIP**. Es mejor clonar el repositorio con `git clone` en lugar de descargar el ZIP del código fuente.

**¿Por qué?**

Cuando se descarga un .zip desde internet y se descomprime en Windows, el sistema operativo puede marcar muchos archivos con el sistema de seguridad llamado: `Mark of the Web (MOTW)`.

Esto hace que Windows considere los archivos como: `"archivos provenientes de internet"` 