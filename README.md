# DopaVoxel

Ce dépôt Github contient le code source du moteur 3D que j'ai développé en 2014, écrit en C# avec la librairie monogame. Ce moteur permet l'affichage d'un monde représenté sous forme de cubes, comme dans le célèbre jeu Minecraft.

Le monde est infini et se charge au fur et à mesure que le joueur avance. Contrairement au jeu original, la topologie du terrain n'est pas générée grâce à un bruit de perlin, mais plutôt à partir de différentes cartes de hauteur mixées entre elles.

Le monde est divisé en morceaux (chunks) de 16x16x256 blocs. Ces morceaux sont générés dans un thread séparé, ce qui permet d'optimiser les performances du moteur. J'ai également tenté d'implémenter un système de gestion de la lumière, mais le sujet s'est avéré plus complexe que prévu.

En conclusion, je suis assez satisfait du rendu obtenu par rapport au temps que j'y ai consacré. Si vous êtes intéressé par ce projet, n'hésitez pas à le télécharger et à y jeter un coup d'œil !

![DopaVoxel](DopaVoxel.jpg?raw=true)
