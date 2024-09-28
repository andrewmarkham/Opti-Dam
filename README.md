# Optimizely DAM integration demo
This repo contains two projects

## Foundation
A version of the 'Foundation' starter solution that references assets from the DAM.

- see `src/Foundation/Features/Blocks/HeroBlock/HeroBlock.cshtml` for an example of the custom tag helper
- see `appSettings.json` for image configuration

To use this site you will need access to the DAM.

There are two schedule jobs that will copy all the assets to the DAM and also update the products to reference the assets in the DAM.

## OptiDAM

The project contains detailed examples demonstrating:
- Image Tag helper which will get tne required renditions from the DAM along with the AltText
- Video Tag helper
- Graph queries to retrieve assets
- Scheduled job to upload assets to the DAM.  This maintains the folder structure from the CMS media library
- Scheduled job to update the products / variants to reference the images from the DAM.

## Licensing
The code in this repository can be reused within your own solution, but I take no responsiblity for any unforseen impact / issues caused.