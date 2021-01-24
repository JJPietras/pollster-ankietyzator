import {PipeTransform, Pipe } from '@angular/core'

@Pipe({
    name: 'keysAdminPipe'
})

export class KeysAdminPipe implements PipeTransform{
  
      transform(items: Key[], searchText: string, selectedCategory: number): Key[] {
        if(!items) return [];
        searchText = searchText.toLowerCase();
            return items.filter( it => {
              return (it.key.toLowerCase().includes(searchText) || it.eMail.toLowerCase().includes(searchText) || !searchText) && (selectedCategory == -1 || selectedCategory == it.userType);
            });
       }
}