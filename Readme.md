# **Bug Legend**
Sebuah game multiplayer/artilery/platformer/action/strategy yang dibangun dalam rangka memenuhi tugas *IF3111 Pengembangan Aplikasi pada Platform Khusus.*

![Bug Legend](https://img0.imguh.com/2019/03/18/Screenshot_4e367c7f81172b2d5.png)

> "Remind me of classic-game *Worm* but with touches of *Fortnite*."

# Spesifikasi
- Platform : Windows - Unity
- Resolusi : 1920 x 1080
- Asset : *free**

# Rancangan Sistem
"Bug Legend" adalah sebuah game bergenre artillery, dan platformer. Game ini terinspirasi dari game klasik yang terkenal yaitu game Worm. Dalam pembuatannya game ini juga mengadaptasi salah satu game shooting battle-royale yang sedang terkenal yaitu Fortnite. 

Dalam game ini player memiliki tujuan mengalahkan player lain dengan cara melempar bug ke player lain. Player bisa melakukan 3 hal, yaitu bergerak (ke kiri, kanan serta melompat), melempar bug, dan membangun platform untuk dinaiki atau sebagai perlindungan, semuanya dilakukan dengan input keyboard. Bug dan platform yang diciptakan player akan saling menghilangkan apabila bertabrakan.

Beberapa aspek lain dari game :
- Sistem akan memanfaatkan hukum gravitasi untuk player dan bug, namun tidak untuk platform. 
- Pergerakan player memanfaatkan input keyboard, dan memanfaatkan animasi player.
- Sound akan ditambahkan sebagai background music saat bermain. 
- Scene yang dibangun : title scene, introduction scene, game scene, result scene.
- Kamera yang digunakan static karena harus menampilkan kedua player dan keseluruhan map pada saat bermain, namun untuk memenuhi syarat pergerakan kamera, ditambahkan fitur zoom dan mengikuti player saat satu player mengalahkan player lain.
- Canvas akan digunakan untuk membuat GUI (start page, end page, etc)
- PlayerPrefs akan digunakan untuk menampung data dan mengirim data dari satu scene ke scene lain (misalnya menentukan player yang menang / kalah dari game scene, untuk ditampilkan di scene result).
- Basis data yang digunakan adalah Firebase Database, digunakan sebagai logger history
- Aplikasi di deploy pada platform windows.

# Fitur
***Multiplayer*** </br>
Game dapat dimainkan secara bersamaan oleh 2 orang atau lebih*.

***Free Move*** </br>
Player dapat menggerakan robot  (representasi player) bebas ke segala arah dengan keyboard [ f ] [ t ] [ h ] untuk player 1; dan [ left arrow ] [ right arrow ] [ up arrow ] untuk player 2. 

***Unlimited Attack*** </br>
Player dapat melemparkan bug secara tidak terbatas (dalam jumlah) dengan menekan [space] atau [right-ctrl]. Bug ini akan menghilangkan platform apabila mengenai platform. Bug juga akan mengalahkan player lain jika mengenainya.

***Unlimited Build*** </br>
Player dapat membangun platform untuk bergerak dan bertahan. Dengan adanya berbagai bentuk platform, player dapat memanfaatkan strategi dan kreatifitas untuk menyerang dan bertahan dalam satu waktu.

***Cinemachine Camera*** </br>
Kamera yang digunakan memanfaatkan Cinemachine untuk mendapatkan kualitas gambar dan transisi yang lebih halus. Player juga dapat memfokuskan kamera untuk mencari player apabila kesulitan menemukan player di game area.


# Asset
| Name  | Author/Source | Free/Pay | Usage |
| ------|---------------|----------|------ |
|Paperdoll-core | [Bird Dog Games](https://assetstore.unity.com/packages/2d/characters/paperdoll-core-81831) | free  |  character |
| God of ink | mpi | copyrighted |Audio asset |
| You won! | Yuki Kajiura | copyrighted |Audio asset |
| AL:Lu | Eliana | copyrighted |Audio asset |
| 音_9RE_eita-zu | Hiroyuki Sawano | copyrighted |Audio asset |
| MKAlieZ | 澤野 弘之 | copyrighted |Audio asset |




# Author
Jessin Donnyson 	13516112    
Find me on : [Github](https://github.com/Jessinra?tab=repositories) [LinkedIn](https://www.linkedin.com/in/jessinra/) [GitLab](https://gitlab.com/Jessinra)


