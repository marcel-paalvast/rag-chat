using 'endpoint.bicep'
import { defaultPrefix } from './main.bicep'

param functionAppPrefix = defaultPrefix

param storageAccountPrefix =  toLower(defaultPrefix)
