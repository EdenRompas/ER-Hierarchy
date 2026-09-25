# ER Hierarchy

**ER Hierarchy** adalah extension untuk Unity Editor yang mempercantik dan menambah fungsi pada window **Hierarchy** bawaan Unity mulai dari pewarnaan baris, ikon komponen, toggle aktif/nonaktif, garis penghubung parent-child, hingga header dan separator berwarna untuk mengorganisir GameObject.

![Tampilan Hierarchy dengan ER Hierarchy aktif](docs/images/hierarchy-overview.png)

## Fitur

- **Alternating Row Colors** baris pada hierarchy diberi warna berselang-seling agar lebih mudah dibaca.
- **Component Icons** menampilkan ikon script/komponen yang terpasang pada tiap GameObject di sisi kanan hierarchy.
- **Active Toggle** checkbox untuk mengaktifkan/menonaktifkan GameObject langsung dari hierarchy, tanpa perlu membuka Inspector.
- **Hierarchy Tree Lines** garis vertikal/horizontal yang menghubungkan parent dan child, sehingga struktur hierarchy lebih mudah dibaca. Garis akan ter-highlight jika ancestor-nya sedang dipilih.
- **ER Header** komponen untuk menambahkan header bergradasi warna sebagai pemisah/label kelompok objek di hierarchy. Tersedia preset warna: `Gray`, `Red`, `Green`, `Blue`, `Yellow`, dan `Custom`.
- **ER Separator** komponen untuk menambahkan garis pemisah sederhana antar kelompok objek di hierarchy.

## Instalasi

1. Salin folder `Scripts` ke dalam folder `Assets` project Unity kamu (atau import sebagai `.unitypackage` jika kamu mendistribusikannya dalam bentuk itu).
2. Pastikan struktur berikut ikut ter-import:
