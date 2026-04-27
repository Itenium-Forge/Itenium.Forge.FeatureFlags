# feature-flags-ui

Module Federation **remote** voor de Itenium Forge POC. Exposed één React component die geladen wordt door de shell op `/feature-flags`.

## Opstarten

```bash
npm install
npm run build && npm run preview   # http://localhost:3001
```

> `npm run dev` werkt niet met Module Federation — gebruik `build + preview`.

## Structuur

```
src/
├── App.tsx       # exposed als featureFlags/App — toont de flags tabel
└── main.tsx      # standalone entry voor lokale ontwikkeling
```

## Exposed modules

| Module | Beschrijving |
|--------|-------------|
| `featureFlags/App` | React component — haalt flags op via `Shell.Api/api/flags` |
