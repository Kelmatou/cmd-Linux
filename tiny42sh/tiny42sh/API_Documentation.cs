﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd_Linux
{
    class API_Documentation
    {
        #region API_UPDATE
        /*PROTOCOL DE CREATION D'UNE NOUVELLE VERSION:
            CMD_LINUX_SETUP -> VERSION
            CMD LINUX -> INFORMATIONS DE L'ASSEMBLY -> VERSION DE L'ASSEMBLY & VERSION DES FICHIERS
            EXECUTION -> VERSION (DATE & N°)
            EXECUTION -> EXECUTE_INFO -> EDITEUR & DEV
            GENERER -> GENERER EN TACHE DE FOND -> GENERER (TOUT COCHER)
            NOUVELLE VERSION CREE! */
        #endregion

        #region API_ADD_CMD
        /*PROTOCOL D'AJOUT D'UNE COMMANDE:
            AJOUTER LA COMMANDE DANS LE FICHIER DES STATS
            AJOUTER LA COMMANDE DANS LA VARIABLE STATIC_DATA
            AJOUTER LE KEYWORD
            AJOUTER LE MATCHING KEYWORD - STRING (INTERPRETER)
            AJOUTER LE MATCHING NUMERO COMMANDE - STRING (STATIC DATA)
            (OPTION) AJOUTER LE NUMERO ARG GENIUS AVEC L'EFFET DANS GENIUS CMD
            AJOUTER LA FONCTION D'EXECUTION ET DE DIRECTION VERS CETTE COMMANDE (EXECUTION)
            AJOUTER L'AIDE TOTAL ET INDIVIDUEL
            COMMANDE AJOUTEE! */
        #endregion
    }
}