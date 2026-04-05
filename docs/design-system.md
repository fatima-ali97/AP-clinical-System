# Design System

## Font choice

### Palanquin Dark & Palanquin

- **Display Font**: Palanquin Dark

- **Body**: Palanquin

**Import (Google Fonts)**

```css
@import url("https://fonts.googleapis.com/css2?family=Palanquin+Dark:wght@400;500;600;700&family=Palanquin:wght@300;400;500;600&display=swap");
```

## Color Palette

### Brand / Primary (Teal-Blue)

The core brand colors — used for primary actions, highlights, and key UI chrome.

| Token                     | Hex       | Swatch                                                       |
| ------------------------- | --------- | ------------------------------------------------------------ |
| `--color-brand-primary`   | `#38789E` | ![#38789E](https://via.placeholder.com/16/38789E/38789E.png) |
| `--color-brand-secondary` | `#399B9E` | ![#399B9E](https://via.placeholder.com/16/399B9E/399B9E.png) |
| `--color-brand-light`     | `#BAD4D5` |                                                              |
| `--color-brand-dark`      | `#030303` |                                                              |

---

### Teal Scale

Full scale from lightest tint to darkest shade.

| Token              | Hex       | Step |
| ------------------ | --------- | ---- |
| `--color-teal-50`  | `#F0FBFC` | 50   |
| `--color-teal-100` | `#D9F3F6` | 100  |
| `--color-teal-200` | `#BFEAF0` | 200  |
| `--color-teal-300` | `#9EDCE6` | 300  |
| `--color-teal-400` | `#6CCAD9` | 400  |
| `--color-teal-500` | `#399B9E` | 500  |
| `--color-teal-600` | `#2F8C8F` | 600  |
| `--color-teal-700` | `#267276` | 700  |
| `--color-teal-800` | `#1E5A5E` | 800  |
| `--color-teal-900` | `#144244` | 900  |
| `--color-teal-950` | `#0D2E30` | 950  |

---

### Red / Danger Scale

| Token             | Hex       | Step |
| ----------------- | --------- | ---- |
| `--color-red-50`  | `#FEF3F3` | 50   |
| `--color-red-100` | `#FDE2E2` | 100  |
| `--color-red-200` | `#FBCACA` | 200  |
| `--color-red-300` | `#F7A7A7` | 300  |
| `--color-red-400` | `#F27777` | 400  |
| `--color-red-500` | `#E54848` | 500  |
| `--color-red-600` | `#CC3A3A` | 600  |
| `--color-red-700` | `#A82E2E` | 700  |
| `--color-red-800` | `#7F2222` | 800  |
| `--color-red-900` | `#5A1717` | 900  |
| `--color-red-950` | `#3A0F0F` | 950  |

---

### Slate / Neutral Scale

| Token               | Hex       | Step |
| ------------------- | --------- | ---- |
| `--color-slate-50`  | `#F8FAFC` | 50   |
| `--color-slate-100` | `#F1F5F9` | 100  |
| `--color-slate-200` | `#E2E8F0` | 200  |
| `--color-slate-300` | `#CBD5E1` | 300  |
| `--color-slate-400` | `#94A3B8` | 400  |
| `--color-slate-500` | `#64748B` | 500  |
| `--color-slate-600` | `#475569` | 600  |
| `--color-slate-700` | `#334155` | 700  |
| `--color-slate-800` | `#1E293B` | 800  |
| `--color-slate-900` | `#0F172A` | 900  |
| `--color-slate-950` | `#020617` | 950  |

---

### Indigo Scale

| Token                | Hex       | Step |
| -------------------- | --------- | ---- |
| `--color-indigo-50`  | `#EEF2FF` | 50   |
| `--color-indigo-100` | `#E0E7FF` | 100  |
| `--color-indigo-200` | `#C7D2FE` | 200  |
| `--color-indigo-300` | `#A5B4FC` | 300  |
| `--color-indigo-400` | `#818CF8` | 400  |
| `--color-indigo-500` | `#4F6DFF` | 500  |
| `--color-indigo-600` | `#3F5BDB` | 600  |
| `--color-indigo-700` | `#334BB8` | 700  |
| `--color-indigo-800` | `#283C94` | 800  |
| `--color-indigo-900` | `#1E2E73` | 900  |
| `--color-indigo-950` | `#141F52` | 950  |

---

### Blue Scale

| Token              | Hex       | Step |
| ------------------ | --------- | ---- |
| `--color-blue-50`  | `#EAF2FD` | 50   |
| `--color-blue-100` | `#D5E6FB` | 100  |
| `--color-blue-200` | `#C1D9FA` | 200  |
| `--color-blue-300` | `#ACCCF8` | 300  |
| `--color-blue-400` | `#97C0F6` | 400  |
| `--color-blue-500` | `#82B3F4` | 500  |
| `--color-blue-600` | `#6DA6F2` | 600  |
| `--color-blue-700` | `#5999F1` | 700  |
| `--color-blue-800` | `#448DEF` | 800  |
| `--color-blue-900` | `#3D7FD7` | 900  |

---

### Gray Scale (Subtle)

Very light near-white steps for backgrounds and surfaces.

| Token              | Hex       | Step |
| ------------------ | --------- | ---- |
| `--color-gray-0`   | `#FDFDFE` | 0    |
| `--color-gray-25`  | `#FCFDFD` | 25   |
| `--color-gray-50`  | `#FBFCFD` | 50   |
| `--color-gray-75`  | `#FBFBFC` | 75   |
| `--color-gray-100` | `#FAFAFB` | 100  |
| `--color-gray-125` | `#F9F9FB` | 125  |
| `--color-gray-150` | `#F8F9FA` | 150  |
| `--color-gray-175` | `#F7F8FA` | 175  |
| `--color-gray-200` | `#F6F7F9` | 200  |
| `--color-gray-225` | `#F5F6F8` | 225  |

---

### Steel Blue Scale

| Token               | Hex       | Step |
| ------------------- | --------- | ---- |
| `--color-steel-50`  | `#E6F0F5` | 50   |
| `--color-steel-100` | `#CCE0EB` | 100  |
| `--color-steel-200` | `#99C2D7` | 200  |
| `--color-steel-300` | `#66A3C3` | 300  |
| `--color-steel-400` | `#38789E` | 400  |
| `--color-steel-500` | `#2F647F` | 500  |
| `--color-steel-600` | `#265061` | 600  |
| `--color-steel-700` | `#1D3C45` | 700  |
| `--color-steel-800` | `#14282E` | 800  |
| `--color-steel-900` | `#0D1A1F` | 900  |

---

### Cyan / Turquoise Scale

| Token              | Hex       | Step |
| ------------------ | --------- | ---- |
| `--color-cyan-50`  | `#D1F7F5` | 50   |
| `--color-cyan-100` | `#BAF3F0` | 100  |
| `--color-cyan-200` | `#A3EFEB` | 200  |
| `--color-cyan-300` | `#8DEBE6` | 300  |
| `--color-cyan-400` | `#76E7E1` | 400  |
| `--color-cyan-500` | `#5FE3DC` | 500  |
| `--color-cyan-600` | `#48DFD7` | 600  |
| `--color-cyan-700` | `#31DBD2` | 700  |
| `--color-cyan-800` | `#1AD7CD` | 800  |
| `--color-cyan-900` | `#17C2B9` | 900  |

---

### Green / Success Scale

| Token               | Hex       | Step |
| ------------------- | --------- | ---- |
| `--color-green-50`  | `#E9F7EF` | 50   |
| `--color-green-100` | `#D4EFDF` | 100  |
| `--color-green-200` | `#BEE7CF` | 200  |
| `--color-green-300` | `#A9DFBF` | 300  |
| `--color-green-400` | `#93D7B0` | 400  |
| `--color-green-500` | `#7DCEA0` | 500  |
| `--color-green-600` | `#68C690` | 600  |
| `--color-green-700` | `#52BE80` | 700  |
| `--color-green-800` | `#3DB670` | 800  |
| `--color-green-900` | `#37A465` | 900  |

---

### Semantic / UI Colors

Standalone accent and status colors.

| Token                 | Hex       | Role             |
| --------------------- | --------- | ---------------- |
| `--color-ui-teal`     | `#4BADBC` | UI Teal accent   |
| `--color-ui-danger`   | `#EF1E1E` | Danger / Error   |
| `--color-ui-indigo`   | `#3538CD` | Indigo accent    |
| `--color-ui-blue`     | `#2F80ED` | Blue accent      |
| `--color-ui-orange`   | `#E04F16` | Orange / Warning |
| `--color-ui-pink`     | `#DD2590` | Pink accent      |
| `--color-ui-purple`   | `#800080` | Purple accent    |
| `--color-ui-green`    | `#27AE60` | Success green    |
| `--color-ui-teal-alt` | `#0E9384` | Teal alt         |

---

### Semantic Background Tints

| Token                 | Hex       | Pair with             |
| --------------------- | --------- | --------------------- |
| `--color-bg-teal`     | `#E9F8FB` | `--color-ui-teal`     |
| `--color-bg-danger`   | `#FEF4F4` | `--color-ui-danger`   |
| `--color-bg-neutral`  | `#E7E8EB` | Neutral UI            |
| `--color-bg-indigo`   | `#EDEDFB` | `--color-ui-indigo`   |
| `--color-bg-blue`     | `#F4F9FE` | `--color-ui-blue`     |
| `--color-bg-white`    | `#FEFEFE` | Pure white            |
| `--color-bg-orange`   | `#FDF6F3` | `--color-ui-orange`   |
| `--color-bg-pink`     | `#FDF4F9` | `--color-ui-pink`     |
| `--color-bg-slate`    | `#ECEDF7` | Slate surfaces        |
| `--color-bg-purple`   | `#F9F2F9` | `--color-ui-purple`   |
| `--color-bg-cyan`     | `#E8FBFA` | `--color-ui-teal-alt` |
| `--color-bg-green`    | `#F4FBF7` | `--color-ui-green`    |
| `--color-bg-teal-alt` | `#E9F5F4` | Teal alt bg           |

---

### Yellow / Warning Scale

| Token                | Hex       | Step |
| -------------------- | --------- | ---- |
| `--color-yellow-50`  | `#FCF8EB` | 50   |
| `--color-yellow-100` | `#F9F1D8` | 100  |
| `--color-yellow-200` | `#F6EAC4` | 200  |
| `--color-yellow-300` | `#F3E3B1` | 300  |
| `--color-yellow-400` | `#F1DC9D` | 400  |
| `--color-yellow-500` | `#EED589` | 500  |
| `--color-yellow-600` | `#EBCE76` | 600  |
| `--color-yellow-700` | `#E8C762` | 700  |
| `--color-yellow-800` | `#E5C04F` | 800  |
| `--color-yellow-900` | `#CEAD47` | 900  |

---

## CSS Custom Properties

Paste this `:root` block into your global stylesheet.
