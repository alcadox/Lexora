# 🧠 Proyecto Lexora

### Proyecto del ciclo DAM – Lenguaje C# (.NET / Visual Studio)

---

## 👥 Equipo de desarrollo

- **Alex** 🧩
- **Avril** 💻 
- **Aaron** ⚙️

---

## 🏗️ Estructura de ramas

| Rama | Descripción |
|------|--------------|
| `main` | Versión final y 100% estable del proyecto. Solo se actualiza cuando todo está probado. |
| `desarrollo` | Rama intermedia donde se integran los cambios de las ramas personales para probar su funcionamiento. |
| `alex`, `avril`, `aaron` | Ramas personales de trabajo. Cada uno desarrolla sus partes aquí. |

> ⚠️ **Regla de oro:**  
> Nadie toca directamente `main` ni `desarrollo`.  
> Todo el trabajo se hace en la rama personal → luego se sube a `desarrollo` → y finalmente a `main`.

---

## ⚙️ Configuración inicial del proyecto

### 1️⃣ Clonar el repositorio

```bash
git clone https://github.com/tuusuario/Lexora.git
cd Lexora
```

### 2️⃣ Cambiar a tu rama personal

```bash
git checkout alex
```

*(Cambia “alex” por tu nombre según tu rama personal: `avril`, `aaron`, etc.)*

---

## 💻 Flujo de trabajo

### 🔹 1. Trabajar en tu rama personal
1. Cambia a tu rama:
   ```bash
   git checkout alex
   ```
2. Actualiza la rama antes de empezar:
   ```bash
   git pull origin alex
   ```
3. Trabaja en tu parte del proyecto desde Visual Studio.  
4. Guarda tus cambios y súbelos:
   ```bash
   git add .
   git commit -m "Descripción del cambio"
   git push origin alex
   ```

---

### 🔹 2. Pasar tus cambios a la rama `desarrollo`

Cuando tu parte esté probada y funcional:

```bash
git checkout desarrollo
git pull origin desarrollo
git merge alex
git push origin desarrollo
```

O, si prefieres hacerlo desde GitHub:

1. Ve al repositorio en GitHub.  
2. Pulsa **Compare & pull request** desde tu rama (`alex`) hacia `desarrollo`.  
3. Añade una descripción del cambio.  
4. Envía la solicitud y espera a que se revise/acepte.

---

### 🔹 3. Pasar de `desarrollo` a `main`

Solo se hace cuando el proyecto está completamente probado.

```bash
git checkout main
git pull origin main
git merge desarrollo
git push origin main
```

> Normalmente este paso lo hará **Alex** (dueño del repositorio de git).

---

## ⚔️ Resolver conflictos (merge conflicts)

Si dos personas modifican el mismo archivo, puede que Git marque un conflicto.

Ejemplo de conflicto:

```
<<<<<<< HEAD
(código de tu rama)
=======
(código de la otra rama)
>>>>>>> desarrollo
```

Solución:
1. Abre el archivo en Visual Studio.  
2. Elimina las marcas `<<<<<<<`, `=======`, `>>>>>>>` y deja solo el código correcto.  
3. Guarda y ejecuta:
   ```bash
   git add .
   git commit -m "Conflicto resuelto en [archivo]"
   git push
   ```

---

## 🧱 Comandos útiles

| Acción | Comando |
|--------|----------|
| Ver ramas disponibles | `git branch -a` |
| Crear una nueva rama | `git checkout -b nombre_rama` |
| Cambiar de rama | `git checkout nombre_rama` |
| Ver cambios pendientes | `git status` |
| Añadir todos los archivos | `git add .` |
| Guardar los cambios | `git commit -m "mensaje"` |
| Subir cambios al remoto | `git push origin nombre_rama` |
| Bajar cambios del remoto | `git pull origin nombre_rama` |
| Hacer merge | `git merge rama_a_unir` |

---

## 🧩 Consejos para el equipo

- **Actualiza tu rama antes de trabajar** (`git pull`) para evitar conflictos.
- **Haz commits pequeños y con sentido.**
  - ✅ Bien: "Añadido formulario de login"
  - 🚫 Mal: "Cambios varios"
- **No mezcles código incompleto en desarrollo o main.**
- **Usad Pull Requests en GitHub** para mantener un control visual de los cambios.
- **Comunicad los merges grandes** en el grupo antes de hacerlos.

---

## 🔁 Esquema del flujo de trabajo

```
       ┌───────────┐
       │   main    │ ← versión final (solo código 100% funcional)
       └─────┬─────┘
             ↑ merge
       ┌───────────┐
       │ desarrollo│ ← pruebas e integración
       └─────┬─────┘
             ↑ merge
 ┌───────┬───────┬───────┐
 │ alex  │ avril │ aaron │ ← ramas personales
 └───────┴───────┴───────┘
```

---

## 🧰 Herramientas recomendadas

- **Visual Studio 2022**
- **Git Bash** o **Git integrado en Visual Studio**
- **GitHub Desktop** (opcional, para trabajar visualmente con ramas)
- **Extensión GitLens** (para ver quién hizo cada cambio en el código)

---

## 📜 Licencia

Proyecto académico desarrollado en el marco del **Grado Superior DAM – 2025**  
© Equipo Lexora
